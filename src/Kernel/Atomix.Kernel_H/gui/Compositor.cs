/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Atom GUI Compositor Class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomixilc.Lib;

using Atomix.Kernel_H.IO;
using Atomix.Kernel_H.Lib;
using Atomix.Kernel_H.Core;
using Atomix.Kernel_H.Lib.Cairo;
using Atomix.Kernel_H.Drivers.Video;
using Atomix.Kernel_H.Drivers.Input;

namespace Atomix.Kernel_H.Gui
{
    internal static class Compositor
    {
        internal const int PACKET_SIZE = 48;

        static IList<Pipe> Clients;
        static IList<Window> Windows;
        static IQueue<uint> RedrawRects;

        static uint WindowsLock;
        static uint RedrawRectsLock;

        internal static Pipe Server;

        static uint MainSurface;
        static uint MouseSurface;
        static uint VideoSurface;

        static uint MainContext;
        static uint VideoContext;

        internal unsafe static void Setup(Process aParent)
        {
            Server = new Pipe(PACKET_SIZE, 1000);

            Clients = new IList<Pipe>();
            Windows = new IList<Window>();
            RedrawRects = new IQueue<uint>();

            int stride = Cairo.FormatStrideForWidth(VBE.Xres, ColorFormat.ARGB32);
            MouseSurface = Cairo.ImageSurfaceFromPng(Native.GetContentAddress("disk0/boot.png\0")); //Cairo.ImageSurfaceCreateForData(4 * 32, 32, 32, ColorFormat.ARGB32, GetMouseBitamp());
            MainSurface = Cairo.ImageSurfaceCreateForData(stride, VBE.Yres, VBE.Xres, ColorFormat.ARGB32, VBE.SecondaryBuffer);
            VideoSurface = Cairo.ImageSurfaceCreateForData(stride, VBE.Yres, VBE.Xres, ColorFormat.ARGB32, VBE.VirtualFrameBuffer);

            MainContext = Cairo.Create(MainSurface);
            VideoContext = Cairo.Create(VideoSurface);

            new Thread(aParent, HandleMouse, Heap.kmalloc(0x1000) + 0x1000, 0x1000).Start();
            new Thread(aParent, HandleRequest, Heap.kmalloc(0x1000) + 0x1000, 0x1000).Start();
            new Thread(aParent, Renderer, Heap.kmalloc(0x10000) + 0x10000, 0x10000).Start();
        }

        internal static int CreateConnection(Pipe aClient)
        {
            if (aClient.PacketSize != PACKET_SIZE) return -1;

            Clients.Add(aClient);
            return (Clients.Count - 1);
        }

        static bool MouseLeftBtn, MouseRightBtn, MouseMiddleBtn;

        static int Mouse_X, Mouse_Y;
        private static unsafe void Renderer()
        {
            int tmp_mouse_X, tmp_mouse_Y;
            int old_mouse_X = -1, old_mouse_Y = -1;
            var MouseBuffer = GetMouseBitamp();

            bool update;
            while (true)
            {
                tmp_mouse_X = Mouse_X;
                tmp_mouse_Y = Mouse_Y;
                update = false;

                Cairo.Save(MainContext);
                Cairo.Save(VideoContext);

                if (tmp_mouse_X != old_mouse_X || tmp_mouse_Y != old_mouse_Y)
                {
                    update = true;
                    Cairo.Rectangle(32, 32, old_mouse_Y, old_mouse_X, MainContext);
                    Cairo.Rectangle(32, 32, old_mouse_Y, old_mouse_X, VideoContext);
                }

                old_mouse_X = tmp_mouse_X;
                old_mouse_Y = tmp_mouse_Y;

                var queue = RedrawRects;
                Monitor.AcquireLock(ref RedrawRectsLock);
                while (queue.Count > 0)
                {
                    var rect = (Rect*)queue.Dequeue();

                    Cairo.Rectangle(rect->Height, rect->Width, rect->Y, rect->X, MainContext);
                    Cairo.Rectangle(rect->Height, rect->Width, rect->Y, rect->X, VideoContext);

                    Libc.free((uint)rect);

                    update = true;
                }
                Monitor.ReleaseLock(ref RedrawRectsLock);

                if (update)
                {
                    Cairo.Clip(MainContext);
                    var list = Windows;
                    Monitor.AcquireLock(ref WindowsLock);
                    int count = list.Count;
                    for (int index = 0; index < count; index++)
                    {
                        var win = list[index];
                        if (win.Z < 0) continue;

                        Cairo.TranslateTo(win.Y, win.X, MainContext);
                        Cairo.SetSourceSurface(0, 0, win.Surface, MainContext);
                        Cairo.Paint(MainContext);
                    }
                    Monitor.ReleaseLock(ref WindowsLock);
                }

                if (update)
                {
                    Cairo.Clip(VideoContext);
                    Cairo.MoveTo(0, 0, VideoContext);
                    Cairo.SetOperator(Operator.Over, VideoContext);
                    Cairo.SetSourceSurface(0, 0, MouseSurface, VideoContext);
                    Cairo.Paint(VideoContext);

                    Lib.Graphic.Surface.CopyToBuffer(VBE.VirtualFrameBuffer, MouseBuffer, old_mouse_X, old_mouse_Y, VBE.Xres, VBE.Yres, 0, 0, 32, 32, 32);
                }

                Cairo.Restore(MainContext);
                Cairo.Restore(VideoContext);
            }
        }

        public static unsafe uint GetMouseBitamp()
        {
            var aBuffer = (byte*)Heap.kmalloc(32 * 32 * 4);

            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    int add = ((j * 32) + i) * 4;
                    aBuffer[add + 0] = 0xFF;
                    aBuffer[add + 1] = 0x55;
                    aBuffer[add + 2] = 0xAA;
                    aBuffer[add + 3] = 0xFF;
                }
            }
            return (uint)aBuffer;
        }

        private static unsafe void HandleMouse()
        {
            var Packet = new byte[4];
            var data = new byte[PACKET_SIZE];
            var request = (GuiRequest*)Native.GetContentAddress(data);

            request->Type = RequestType.MouseEvent;

            var mouseRequest = (MouseData*)request;
            while (true)
            {
                Mouse.MousePipe.Read(Packet);

                if (Packet[0] != Mouse.MOUSE_MAGIC)
                    continue;

                mouseRequest->Button = Packet[1];
                mouseRequest->Xpos = Packet[2];
                mouseRequest->Ypos = Packet[3];

                Server.Write(data);
            }
        }

        private static unsafe void HandleRequest()
        {
            var packet = new byte[PACKET_SIZE];

            while(true)
            {
                Server.Read(packet);

                var request = (GuiRequest*)Native.GetContentAddress(packet);

                if (request->ClientID >= Clients.Count)
                    request->Error = ErrorType.BadRequest;

                if (request->Error == ErrorType.None)
                {
                    switch (request->Type)
                    {
                        case RequestType.MouseEvent:
                            {
                                var mouse_request = (MouseData*)request;
                                int btn = mouse_request->Button;

                                if ((btn & 0x10) == 0)
                                    Mouse_X += mouse_request->Xpos;
                                else
                                    Mouse_X -= (mouse_request->Xpos ^ 0xFF);

                                if ((btn & 0x20) == 0)
                                    Mouse_Y -= mouse_request->Ypos;
                                else
                                    Mouse_Y += (mouse_request->Ypos ^ 0xFF);

                                MouseLeftBtn = (btn & 0x1) != 0;
                                MouseRightBtn = (btn & 0x2) != 0;
                                MouseMiddleBtn = (btn & 0x4) != 0;

                                if (Mouse_X < 0 || Mouse_X > VBE.Xres)
                                    Mouse_X = 0;

                                if (Mouse_Y < 0 || Mouse_Y > VBE.Yres)
                                    Mouse_Y = 0;
                            }
                            break;
                        case RequestType.NewWindow:
                            {
                                var winRequest = (NewWindow*)request;

                                if (winRequest->Width == 0 || winRequest->Height == 0)
                                {
                                    request->Error = ErrorType.BadParameters;
                                    break;
                                }

                                var newWindow = new Window
                                    (
                                        request->ClientID,
                                        winRequest->X,
                                        winRequest->Y,
                                        winRequest->Width,
                                        winRequest->Height
                                    );

                                Marshal.Copy(winRequest->Buffer, newWindow.HashID, newWindow.HashID.Length);

                                Monitor.AcquireLock(ref WindowsLock);
                                Windows.Add(newWindow);
                                Monitor.ReleaseLock(ref WindowsLock);
                            }
                            break;
                        case RequestType.Redraw:
                            {
                                var redraw = (Redraw*)request;

                                int id = redraw->WindowID;
                                if (id < 0 || id >= Windows.Count)
                                {
                                    request->Error = ErrorType.BadParameters;
                                    break;
                                }

                                var win = Windows[id];

                                var rect = (Rect*)Libc.malloc((uint)sizeof(Rect));
                                rect->X = redraw->X + win.X;
                                rect->Y = redraw->Y + win.Y;
                                rect->Width = redraw->Width;
                                rect->Height = redraw->Height;

                                Monitor.AcquireLock(ref RedrawRectsLock);
                                RedrawRects.Enqueue((uint)rect);
                                Monitor.ReleaseLock(ref RedrawRectsLock);
                            }
                            break;
                        case RequestType.WindowMove:
                            {

                            }
                            break;
                        default:
                            request->Error = ErrorType.BadFunction;
                            break;
                    }
                }
                Clients[request->ClientID].Write(packet, false);
            }
        }
    }
}
