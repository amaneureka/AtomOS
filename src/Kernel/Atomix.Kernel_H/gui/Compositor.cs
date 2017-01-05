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

        static Window ActiveWindow;
        static IList<Pipe> Clients;
        static IList<Window> Windows;
        static IQueue<uint> RedrawRects;

        internal static Pipe Server;

        /* Locks */
        static uint WindowsLock;
        static uint RedrawRectsLock;

        /* Mouse Surfaces */
        static uint MouseSurface;
        static uint MouseIdleSurface;
        static uint MouseHelpSurface;
        static uint MouseClipSurface;

        /* Other Surfaces */
        static uint MainSurface;
        static uint VideoSurface;

        /* Cairo Contexts */
        static uint MainContext;
        static uint VideoContext;

        internal unsafe static void Setup(Process aParent)
        {
            Server = new Pipe(PACKET_SIZE, 1000);

            Clients = new IList<Pipe>();
            Windows = new IList<Window>();
            RedrawRects = new IQueue<uint>();

            /* Mouse Surfaces */
            MouseIdleSurface = Cairo.ImageSurfaceFromPng(Marshal.C_String("disk0/cursor_idle.png"));
            MouseHelpSurface = Cairo.ImageSurfaceFromPng(Marshal.C_String("disk0/cursor_help.png"));
            MouseClipSurface = Cairo.ImageSurfaceFromPng(Marshal.C_String("disk0/cursor_clip.png"));
            MouseSurface = MouseIdleSurface;

            int stride = Cairo.FormatStrideForWidth(VBE.Xres, ColorFormat.ARGB32);
            MainSurface = Cairo.ImageSurfaceCreateForData(stride, VBE.Yres, VBE.Xres, ColorFormat.ARGB32, VBE.SecondaryBuffer);
            VideoSurface = Cairo.ImageSurfaceCreateForData(stride, VBE.Yres, VBE.Xres, ColorFormat.ARGB32, VBE.VirtualFrameBuffer);

            MainContext = Cairo.Create(MainSurface);
            VideoContext = Cairo.Create(VideoSurface);

            new Thread(aParent, Renderer).Start();
            new Thread(aParent, HandleMouse).Start();
            new Thread(aParent, HandleRequest).Start();
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
                    Cairo.Rectangle(32, 32, tmp_mouse_Y, tmp_mouse_X, VideoContext);
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

                    Cairo.TranslateTo(0, 0, VideoContext);
                    Cairo.SetOperator(Operator.Source, VideoContext);
                    Cairo.SetSourceSurface(0, 0, MainSurface, VideoContext);
                    Cairo.Paint(VideoContext);

                    Cairo.TranslateTo(old_mouse_Y, old_mouse_X, VideoContext);
                    Cairo.SetOperator(Operator.Over, VideoContext);
                    Cairo.SetSourceSurface(0, 0, MouseSurface, VideoContext);
                    Cairo.Paint(VideoContext);
                }

                Cairo.Restore(MainContext);
                Cairo.Restore(VideoContext);
            }
        }

        private static unsafe void HandleMouse()
        {
            var Packet = new byte[4];
            var aData = new byte[PACKET_SIZE];
            var request = (GuiRequest*)aData.GetDataOffset();

            request->Type = RequestType.MouseEvent;

            var mouseRequest = (MouseData*)request;
            while (true)
            {
                Mouse.MousePipe.Read(Packet);

                if (Packet[0] != Mouse.MOUSE_MAGIC)
                    continue;

                int btn = Packet[1];
                int x = Packet[2];
                int y = Packet[3];

                mouseRequest->Button = btn;

                if ((btn & 0x10) == 0)
                    x = (x << 1);
                else
                    x = -((x ^ 0xFF) << 1);

                if ((btn & 0x20) == 0)
                    y = -(y << 1);
                else
                    y = ((y ^ 0xFF) << 1);

                mouseRequest->Xpos = x;
                mouseRequest->Ypos = y;

                Server.Write(aData);
            }
        }

        private static unsafe void HandleRequest()
        {
            var xData = new byte[PACKET_SIZE];

            while(true)
            {
                Server.Read(xData);

                var request = (GuiRequest*)xData.GetDataOffset();

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

                                /* calculate new mouse position */
                                int x = Mouse_X + mouse_request->Xpos;
                                int y = Mouse_Y + mouse_request->Ypos;

                                /* bound mouse position */
                                if (x < 0) x = 0;
                                if (x > VBE.Xres) x = VBE.Xres;

                                if (y < 0) y = 0;
                                if (y > VBE.Yres) y = VBE.Yres;

                                /* button details */
                                MouseLeftBtn = (btn & 0x1) != 0;
                                MouseRightBtn = (btn & 0x2) != 0;
                                MouseMiddleBtn = (btn & 0x4) != 0;

                                /* pass information to active window */
                                if (ActiveWindow == null)
                                    continue;

                                mouse_request->Xpos = x - Mouse_X;
                                mouse_request->Ypos = y - Mouse_Y;
                                mouse_request->WindowID = ActiveWindow.ID;
                                request->ClientID = ActiveWindow.ClientID;

                                Mouse_X = x;
                                Mouse_Y = y;
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

                                Marshal.Copy(newWindow.HashID, winRequest->Buffer, newWindow.HashID.Length);

                                Monitor.AcquireLock(ref WindowsLock);
                                newWindow.ID = Windows.Count;
                                winRequest->WindowID = Windows.Count;
                                Windows.Add(newWindow);
                                Monitor.ReleaseLock(ref WindowsLock);
                                ActiveWindow = newWindow;
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
                                var winmove = (WindowMove*)request;

                                int id = winmove->WindowID;
                                if (id < 0 || id >= Windows.Count)
                                {
                                    request->Error = ErrorType.BadParameters;
                                    break;
                                }

                                var win = Windows[id];

                                var rect = (Rect*)Libc.malloc((uint)sizeof(Rect));
                                rect->X = win.X;
                                rect->Y = win.Y;
                                rect->Width = win.Width;
                                rect->Height = win.Height;

                                var rect2 = (Rect*)Libc.malloc((uint)sizeof(Rect));
                                win.X += winmove->RelX;
                                win.Y += winmove->RelY;

                                rect2->X = win.X;
                                rect2->Y = win.Y;
                                rect2->Width = win.Width;
                                rect2->Height = win.Height;

                                Monitor.AcquireLock(ref RedrawRectsLock);
                                RedrawRects.Enqueue((uint)rect);
                                RedrawRects.Enqueue((uint)rect2);
                                Monitor.ReleaseLock(ref RedrawRectsLock);
                            }
                            break;
                        case RequestType.RandomRequest:
                            {
                                var random = (RandomRequest*)request;

                                if (ActiveWindow == null || random->WindowID != ActiveWindow.ID)
                                {
                                    request->Error = ErrorType.BadRequest;
                                    break;
                                }

                                switch(random->MouseIcon)
                                {
                                    case MouseIcon.Idle:
                                        MouseSurface = MouseIdleSurface;
                                        break;
                                    case MouseIcon.Help:
                                        MouseSurface = MouseHelpSurface;
                                        break;
                                    case MouseIcon.Clipboard:
                                        MouseSurface = MouseClipSurface;
                                        break;
                                    case MouseIcon.None: break;
                                    default:
                                        request->Error = ErrorType.BadRequest;
                                        break;
                                }

                                switch(random->WindowState)
                                {
                                    default:
                                        request->Error = ErrorType.BadRequest;
                                        break;
                                }
                            }
                            break;
                        default:
                            request->Error = ErrorType.BadFunction;
                            break;
                    }
                }

                request->HashID = 0;
                Clients[request->ClientID].Write(xData, false);
            }
        }
    }
}
