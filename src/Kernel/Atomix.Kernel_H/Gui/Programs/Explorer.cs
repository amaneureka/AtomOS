using System;
using System.Collections.Generic;

using Atomixilc.Lib;

using Atomix.Kernel_H.IO;
using Atomix.Kernel_H.Lib;
using Atomix.Kernel_H.Core;
using Atomix.Kernel_H.Arch.x86;
using Atomix.Kernel_H.Lib.Cairo;
using Atomix.Kernel_H.Drivers.Video;

namespace Atomix.Kernel_H.Gui.Programs
{
    internal unsafe class Explorer
    {
        static Pipe Client;

        static int DesktopID;
        static int TaskbarID;

        static uint DesktopSurface;
        static uint TaskbarSurface;

        static uint DesktopContext;
        static uint TaskbarContext;

        internal static void Init(Pipe aClient)
        {
            Client = aClient;

            Desktop();
            Taskbar();
        }

        private static void Desktop()
        {
            var Data = new byte[Compositor.PACKET_SIZE];
            var Request = (GuiRequest*)Data.GetDataOffset();

            Request->ClientID = 0;
            Request->Type = RequestType.NewWindow;

            var Window = (NewWindow*)Request;
            Window->X = 0;
            Window->Y = 0;
            Window->Width = VBE.Xres;
            Window->Height = VBE.Yres;

            Compositor.Server.Write(Data);
            Client.Read(Data);

            string HashCode = new string(Window->Buffer);
            var aBuffer = SHM.Obtain(HashCode, 0, false);
            DesktopID = Window->WindowID;

            uint surface = Cairo.ImageSurfaceCreateForData(Window->Width * 4, Window->Height, Window->Width, ColorFormat.ARGB32, aBuffer);
            uint context = Cairo.Create(surface);

            uint wallpaper = Cairo.ImageSurfaceFromPng(Marshal.C_String("disk0/wallpaper.png"));
            Cairo.SetSourceSurface(0, 0, wallpaper, context);
            Cairo.Paint(context);
            Cairo.SurfaceDestroy(wallpaper);

            Request->Type = RequestType.Redraw;
            var Redraw = (Redraw*)Request;
            Redraw->WindowID = DesktopID;
            Redraw->X = 0;
            Redraw->Y = 0;
            Redraw->Width = VBE.Xres;
            Redraw->Height = VBE.Yres;
            Compositor.Server.Write(Data);

            DesktopSurface = surface;
            DesktopContext = context;
        }

        private static void Taskbar()
        {
            var Data = new byte[Compositor.PACKET_SIZE];
            var Request = (GuiRequest*)Data.GetDataOffset();

            Request->ClientID = 0;
            Request->Type = RequestType.NewWindow;

            var Window = (NewWindow*)Request;
            Window->X = 0;
            Window->Y = 0;
            Window->Width = VBE.Xres;
            Window->Height = 30;

            Compositor.Server.Write(Data);
            Client.Read(Data);

            string HashCode = new string(Window->Buffer);
            var aBuffer = SHM.Obtain(HashCode, 0, false);
            TaskbarID = Window->WindowID;

            uint surface = Cairo.ImageSurfaceCreateForData(Window->Width * 4, Window->Height, Window->Width, ColorFormat.ARGB32, aBuffer);
            uint context = Cairo.Create(surface);

            uint pattern = Cairo.PatternCreateLinear(30, 0, 0, 0);
            Cairo.PatternAddColorStopRgba(0.7, 0.42, 0.42, 0.42, 0, pattern);
            Cairo.PatternAddColorStopRgba(0.6, 0.36, 0.36, 0.36, 0.5, pattern);
            Cairo.PatternAddColorStopRgba(0.7, 0.42, 0.42, 0.42, 1, pattern);

            Cairo.SetOperator(Operator.Over, context);
            Cairo.Rectangle(30, VBE.Xres, 0, 0, context);
            Cairo.SetSource(pattern, context);
            Cairo.Fill(context);

            Cairo.Rectangle(2, VBE.Xres, 30 - 2, 0, context);
            Cairo.SetSourceRGBA(0.7, 0.41, 0.41, 0.41, context);
            Cairo.Fill(context);

            Cairo.PatternDestroy(pattern);
            Cairo.Destroy(context);
            Cairo.SurfaceDestroy(surface);

            Request->Type = RequestType.Redraw;
            var Redraw = (Redraw*)Request;
            Redraw->WindowID = TaskbarID;
            Redraw->X = 0;
            Redraw->Y = 0;
            Redraw->Width = VBE.Xres;
            Redraw->Height = 40;
            Compositor.Server.Write(Data);

            TaskbarSurface = surface;
            TaskbarContext = context;
        }
    }
}
