/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
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
        static IQueue<uint> RedrawRects;

        internal static Pipe Server;

        /* Locks */
        static uint WindowsLock;
        static uint StackingLock;
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

        /* Windows */
        static Window MouseWindow;
        static IList<Window> Windows;
        static IList<Window> Stacking;

        internal unsafe static void Setup(Process aParent)
        {
            Server = new Pipe(PACKET_SIZE, 1000);

            Clients = new IList<Pipe>();
            Windows = new IList<Window>();
            Stacking = new IList<Window>();
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

            new Thread(aParent, HandleRequest).Start();
            new Thread(aParent, HandleMouse).Start();
            new Thread(aParent, Renderer).Start();
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
                    var list = Stacking;
                    Monitor.AcquireLock(ref StackingLock);
                    int count = list.Count;
                    for (int index = 0; index < count; index++)
                    {
                        var win = list[index];
                        Cairo.Save(MainContext);
                        Cairo.Translate(win.Y, win.X, MainContext);
                        Cairo.SetSourceSurface(0, 0, win.Surface, MainContext);
                        Cairo.Paint(MainContext);
                        Cairo.Restore(MainContext);
                    }
                    Monitor.ReleaseLock(ref StackingLock);
                }

                if (update)
                {
                    Cairo.Clip(VideoContext);

                    Cairo.Translate(0, 0, VideoContext);
                    Cairo.SetOperator(Operator.Source, VideoContext);
                    Cairo.SetSourceSurface(0, 0, MainSurface, VideoContext);
                    Cairo.Paint(VideoContext);

                    Cairo.Translate(old_mouse_Y, old_mouse_X, VideoContext);
                    Cairo.SetOperator(Operator.Over, VideoContext);
                    Cairo.SetSourceSurface(0, 0, MouseSurface, VideoContext);
                    Cairo.Paint(VideoContext);
                }

                Cairo.Restore(MainContext);
                Cairo.Restore(VideoContext);

                Task.Switch();
            }
        }

        private static unsafe void HandleMouse()
        {
            var Packet = new byte[4];
            var aData = new byte[PACKET_SIZE];
            var request = (GuiRequest*)aData.GetDataOffset();

            request->Type = RequestType.MouseEvent;

            var mouseRequest = (MouseEvent*)request;
            while (true)
            {
                Mouse.MousePipe.Read(Packet);

                if (Packet[0] != Mouse.MOUSE_MAGIC)
                    continue;

                int btn = Packet[1];
                int x = Packet[2];
                int y = Packet[3];

                if ((btn & 0x10) != 0)
                    x |= ~0xFF;

                if ((btn & 0x20) != 0)
                    y |= ~0xFF;

                mouseRequest->Button = btn;

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

                var Request = (GuiRequest*)xData.GetDataOffset();

                if (Request->ClientID >= Clients.Count)
                    Request->Error = ErrorType.BadRequest;

                if (Request->Error != ErrorType.None)
                {
                    ReplyClient(Request);
                    continue;
                }

                switch (Request->Type)
                {
                    case RequestType.MouseEvent:
                        HandleMouseEvent(Request);
                        break;
                    case RequestType.NewWindow:
                        HandleNewWindow(Request);
                        break;
                    case RequestType.Redraw:
                        HandleRedraw(Request);
                        break;
                    case RequestType.WindowMove:
                        HandleWindowMove(Request);
                        break;
                    case RequestType.DragRequest:
                        HandleDragRequest(Request);
                        break;
                    case RequestType.InfoRequest:
                        HandleInfoRequest(Request);
                        break;
                    default:
                        Debug.Write("Function: %d\n", (int)Request->Type);
                        Request->Error = ErrorType.BadFunction;
                        break;
                }
            }
        }

        private static unsafe void HandleInfoRequest(GuiRequest* aRequest)
        {
            var InfoRequest = (InfoRequest*)aRequest;
            int id = InfoRequest->WindowID;

            if (id < 0 || id >= Windows.Count)
                aRequest->Error = ErrorType.BadParameters;

            if (aRequest->Error != ErrorType.None)
            {
                ReplyClient(aRequest);
                return;
            }

            var win = Windows[id];
            InfoRequest->X = win.X;
            InfoRequest->Y = win.Y;
            InfoRequest->Width = win.Width;
            InfoRequest->Height = win.Height;
            ReplyClient(aRequest);
        }

        private static unsafe void HandleDragRequest(GuiRequest* aRequest)
        {
            var DragRequest = (DragRequest*)aRequest;
            int id = DragRequest->WindowID;

            if (id < 0 || id >= Windows.Count)
                aRequest->Error = ErrorType.BadParameters;

            if (ActiveWindow == null || ActiveWindow.ID != id)
                aRequest->Error = ErrorType.BadRequest;

            if (aRequest->Error != ErrorType.None)
            {
                ReplyClient(aRequest);
                return;
            }

            var win = Windows[id];
            MouseWindow = win;
        }

        private static unsafe void HandleMouseEvent(GuiRequest* aRequest)
        {
            var MouseRequest = (MouseEvent*)aRequest;
            MouseRequest->Function = MouseFunction.None;

            MouseFunction function = 0;
            if (MouseRequest->Xpos != 0 || MouseRequest->Ypos != 0)
                function = MouseFunction.Move;

            /* calculate new mouse position */
            int x = Mouse_X + (MouseRequest->Xpos << 1);
            int y = Mouse_Y - (MouseRequest->Ypos << 1);

            /* bound mouse position */
            if (x < 0) x = 0;
            if (x > VBE.Xres) x = VBE.Xres;

            if (y < 0) y = 0;
            if (y > VBE.Yres) y = VBE.Yres;

            /* Button Status */
            int Button = MouseRequest->Button;
            bool LeftBtn = (Button & 0x1) != 0;
            bool RightBtn = (Button & 0x2) != 0;

            /* stop window movement */
            if (LeftBtn)
            {
                if (MouseWindow != null)
                {
                    var win = MouseWindow;
                    MarkRectange(win.X, win.Y, win.Width, win.Height);
                    MouseWindow.X += x - Mouse_X;
                    MouseWindow.Y += y - Mouse_Y;
                    MarkRectange(win.X, win.Y, win.Width, win.Height);
                }
            }
            else
            {
                MouseWindow = null;
            }

            /* Update Coordinates */
            Mouse_X = x;
            Mouse_Y = y;

            /* Mouse Events to Client */
            if ((LeftBtn && !MouseLeftBtn) || (RightBtn && !MouseRightBtn))
                function |= MouseFunction.KeyDown;

            if ((!LeftBtn && MouseLeftBtn) || (!RightBtn && MouseRightBtn))
                function |= MouseFunction.KeyUp;

            /* button details */
            MouseLeftBtn = LeftBtn;
            MouseRightBtn = RightBtn;
            MouseMiddleBtn = (Button & 0x4) != 0;

            if ((function & MouseFunction.KeyDown) != 0)
            {
                Monitor.AcquireLock(ref StackingLock);

                var list = Stacking;
                int count = list.Count;
                for (int i = count - 1; i >= 0; i--)
                {
                    var win = list[i];
                    if (x < win.X ||
                        y < win.Y ||
                        x > win.X + win.Width ||
                        y > win.Y + win.Height) continue;

                    if (ActiveWindow == win)
                    {
                        function |= MouseFunction.Click;
                        break;
                    }

                    ActiveWindow = win;
                    function |= MouseFunction.Enter;

                    if (i != 0)
                    {
                        for (int j = i + 1; j < count; j++)
                            list[j - 1] = list[j];
                        list[count - 1] = win;

                        MarkRectange(win.X, win.Y, win.Width, win.Height);
                    }
                    break;
                }

                Monitor.ReleaseLock(ref StackingLock);
            }

            if (ActiveWindow == null) return;

            var Window = ActiveWindow;

            if (x < Window.X ||
                y < Window.Y ||
                x > Window.X + Window.Width ||
                y > Window.Y + Window.Height)
                return;

            MouseRequest->WindowID = Window.ID;
            MouseRequest->Xpos = x - Window.X;
            MouseRequest->Ypos = y - Window.Y;
            MouseRequest->Function = function;
            aRequest->ClientID = Window.ClientID;
            ReplyClient(aRequest);
        }

        private static unsafe void HandleWindowMove(GuiRequest* aRequest)
        {
            var WindowRequest = (WindowMove*)aRequest;
            int id = WindowRequest->WindowID;
            if (id < 0 || id >= Windows.Count)
                aRequest->Error = ErrorType.BadParameters;

            if (aRequest->Error != ErrorType.None)
            {
                ReplyClient(aRequest);
                return;
            }

            var win = Windows[id];
            MarkRectange(win.X, win.Y, win.Width, win.Height);
            win.X += WindowRequest->RelX;
            win.Y += WindowRequest->RelY;
            MarkRectange(win.X, win.Y, win.Width, win.Height);
        }

        private static unsafe void HandleNewWindow(GuiRequest* aRequest)
        {
            var WindowRequest = (NewWindow*)aRequest;

            int x = WindowRequest->X;
            int y = WindowRequest->Y;
            int width = WindowRequest->Width;
            int height = WindowRequest->Height;

            if (width <= 0 || height <= 0)
                aRequest->Error = ErrorType.BadParameters;

            if (aRequest->Error != ErrorType.None)
            {
                ReplyClient(aRequest);
                return;
            }

            var window = new Window(aRequest->ClientID, x, y, width, height);
            Marshal.Copy(window.HashID, WindowRequest->Buffer, window.HashID.Length);

            Monitor.AcquireLock(ref WindowsLock);

            int id = Windows.Count;
            window.ID = id;
            WindowRequest->WindowID = id;
            Windows.Add(window);

            Monitor.ReleaseLock(ref WindowsLock);
            ActiveWindow = window;

            Monitor.AcquireLock(ref StackingLock);
            Stacking.Add(window);
            Monitor.ReleaseLock(ref StackingLock);

            ReplyClient(aRequest);
        }

        private static unsafe void HandleRedraw(GuiRequest* aRequest)
        {
            var RedrawRequest = (Redraw*)aRequest;

            int id = RedrawRequest->WindowID;
            int width = RedrawRequest->Width;
            int height = RedrawRequest->Height;

            if (id < 0 || id >= Windows.Count || width <= 0 || height <= 0)
                aRequest->Error = ErrorType.BadParameters;

            if (aRequest->Error != ErrorType.None)
            {
                ReplyClient(aRequest);
                return;
            }

            var Window = Windows[id];
            MarkRectange(RedrawRequest->X + Window.X, RedrawRequest->Y + Window.Y, width, height);
        }

        private static unsafe void MarkRectange(int x, int y, int width, int height)
        {
            var DamageRect = (Rect*)Libc.malloc(sizeof(Rect));
            DamageRect->X = x;
            DamageRect->Y = y;
            DamageRect->Width = width;
            DamageRect->Height = height;

            Monitor.AcquireLock(ref RedrawRectsLock);
            RedrawRects.Enqueue((uint)DamageRect);
            Monitor.ReleaseLock(ref RedrawRectsLock);
        }

        private static unsafe void ReplyClient(GuiRequest* aRequest)
        {
            aRequest->HashID = 0;
            Clients[aRequest->ClientID].Write((byte*)aRequest, false);
        }
    }
}
