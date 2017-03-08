/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Boot Extension Class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomixilc.Lib;
using Atomix.Kernel_H.Lib.Cairo;
using Atomix.Kernel_H.IO;
using Atomix.Kernel_H.Gui;
using Atomix.Kernel_H.Core;
using Atomix.Kernel_H.Devices;
using Atomix.Kernel_H.Arch.x86;
using Atomix.Kernel_H.Drivers.Video;
using Atomix.Kernel_H.IO.FileSystem;

using Atomix.Kernel_H.Lib;
using Atomix.Kernel_H.Drivers.Input;
using Atomix.Kernel_H.Drivers.buses.ATA;

namespace Atomix.Kernel_H
{
    internal class Boot
    {
        internal static int ClientID;
        internal static Pipe SystemClient;

        internal unsafe static void Init()
        {
            Debug.Write("Boot Init()\n");

            #region InitRamDisk
            if (Multiboot.RamDisk != 0)
            {
                var xFileSystem = new RamFileSystem(Multiboot.RamDisk, Multiboot.RamDiskSize);
                if (xFileSystem.IsValid)
                    VirtualFileSystem.MountDevice(null, xFileSystem);
                else
                    throw new Exception("RamDisk Corrupted!");
            }
            else
                throw new Exception("RamDisk not found!");
            #endregion
            #region PS2 Devices
            Keyboard.Setup();
            Mouse.Setup();
            #endregion
            #region Compositor
            SystemClient = new Pipe(Compositor.PACKET_SIZE, 100);
            Compositor.Setup(Scheduler.SystemProcess);
            ClientID = Compositor.CreateConnection(SystemClient);


            #endregion
            #region IDE Devices
            LoadIDE(true, true);
            LoadIDE(false, true);
            #endregion

            //FILE READING TEST
            /*var stream = VirtualFileSystem.GetFile("disk0/gohu-11.bdf");
            if (stream != null)
                Debug.Write(stream.ReadToEnd());
            else
                Debug.Write("File not found!\n");*/
            Gui.Programs.Explorer.Init(SystemClient);
            BootAnimation();

            while (true) ;
        }

        internal static unsafe void BootAnimation()
        {
            var xData = new byte[Compositor.PACKET_SIZE];
            var Request = (GuiRequest*)xData.GetDataOffset();
            Request->ClientID = ClientID;

            DrawWindow(Request, xData);
        }

        private static unsafe void DrawWindow(GuiRequest* request, byte[] xData)
        {
            request->Type = RequestType.NewWindow;
            request->Error = ErrorType.None;
            var window = (NewWindow*)request;
            window->X = 340;
            window->Y = 159;
            window->Width = 600;
            window->Height = 450;

            Compositor.Server.Write(xData);

            SystemClient.Read(xData);
            if (request->Error != ErrorType.None)
            {
                Debug.Write("Error5: %d\n", (int)request->Error);
                return;
            }

            string HashCode = new string(window->Buffer);
            var aBuffer = SHM.Obtain(HashCode, 0, false);
            int winID = window->WindowID;
            Debug.Write("winID: %d\n", winID);

            uint surface = Cairo.ImageSurfaceCreateForData(600 * 4, 450, 600, ColorFormat.ARGB32, aBuffer);
            uint cr = Cairo.Create(surface);

            Cairo.SetOperator(Operator.Over, cr);

            Cairo.Rectangle(450, 600, 0, 0, cr);
            Cairo.SetSourceRGBA(1, 0.41, 0.41, 0.41, cr);
            Cairo.Fill(cr);
            Cairo.Rectangle(446, 596, 2, 2, cr);
            Cairo.SetSourceRGBA(1, 0.87, 0.87, 0.87, cr);
            Cairo.Fill(cr);
            Cairo.Rectangle(410, 580, 30, 10, cr);
            Cairo.SetSourceRGBA(1, 1, 1, 1, cr);
            Cairo.Fill(cr);

            Cairo.SetSourceRGBA(1, 0.41, 0.41, 0.41, cr);
            Cairo.SelectFontFace(FontWeight.Normal, FontSlant.Normal, Marshal.C_String(""), cr);
            Cairo.SetFontSize(15, cr);
            Cairo.MoveTo(18, 215, cr);
            Cairo.ShowText(Marshal.C_String("Atom OS : Installation Guide"), cr);

            Cairo.SelectFontFace(FontWeight.Bold, FontSlant.Normal, Marshal.C_String(""), cr);
            Cairo.MoveTo(18, 580, cr);
            Cairo.ShowText(Marshal.C_String("X"), cr);

            Cairo.SurfaceFlush(surface);
            Cairo.Destroy(cr);
            Cairo.SurfaceDestroy(surface);

            request->Type = RequestType.Redraw;
            var req = (Redraw*)request;
            req->WindowID = winID;
            req->X = 0;
            req->Y = 0;
            req->Width = 600;
            req->Height = 450;
            Compositor.Server.Write(xData);

            Debug.Write("Time: %d\n", Timer.TicksFromStart);
            while(true)
            {
                SystemClient.Read(xData);
                if (request->Error != ErrorType.None) continue;
                if (request->Type != RequestType.MouseEvent) continue;
                var mreq = (MouseEvent*)request;
                if (mreq->WindowID != winID) continue;
                if ((mreq->Function & MouseFunction.Click) != 0)
                {
                    int x = mreq->Xpos;
                    int y = mreq->Ypos;
                    if (y < 40)
                    {
                        request->Type = RequestType.DragRequest;
                        var mv = (DragRequest*)request;
                        mv->WindowID = winID;
                        Compositor.Server.Write(xData);
                    }
                }
            }
        }

        private static unsafe void DrawTaskbar(GuiRequest* request, byte[] xData)
        {
            request->Type = RequestType.NewWindow;
            request->Error = ErrorType.None;
            var taskbar = (NewWindow*)request;
            int height = 30;
            taskbar->X = 0;
            taskbar->Y = 0;
            taskbar->Width = VBE.Xres;
            taskbar->Height = height;

            Compositor.Server.Write(xData);

            SystemClient.Read(xData);
            if (request->Error != ErrorType.None)
            {
                Debug.Write("Error4: %d\n", (int)request->Error);
                return;
            }

            string HashCode = new string(taskbar->Buffer);
            var aBuffer = SHM.Obtain(HashCode, 0, false);
            int winID = taskbar->WindowID;
            Debug.Write("winID: %d\n", winID);

            uint surface = Cairo.ImageSurfaceCreateForData(VBE.Xres * 4, height, VBE.Xres, ColorFormat.ARGB32, aBuffer);
            uint cr = Cairo.Create(surface);

            uint pattern = Cairo.PatternCreateLinear(height, 0, 0, 0);
            Cairo.PatternAddColorStopRgba(0.7, 0.42, 0.42, 0.42, 0, pattern);
            Cairo.PatternAddColorStopRgba(0.6, 0.36, 0.36, 0.36, 0.5, pattern);
            Cairo.PatternAddColorStopRgba(0.7, 0.42, 0.42, 0.42, 1, pattern);

            Cairo.SetOperator(Operator.Over, cr);
            Cairo.Rectangle(height, VBE.Xres, 0, 0, cr);
            Cairo.SetSource(pattern, cr);
            Cairo.Fill(cr);

            Cairo.Rectangle(2, VBE.Xres, height - 2, 0, cr);
            Cairo.SetSourceRGBA(0.7, 0.41, 0.41, 0.41, cr);
            Cairo.Fill(cr);

            Cairo.SetSourceRGBA(1, 1, 1, 1, cr);
            Cairo.SelectFontFace(FontWeight.Bold, FontSlant.Normal, Marshal.C_String(""), cr);
            Cairo.SetFontSize(20, cr);
            Cairo.MoveTo(20, 1215, cr);
            Cairo.ShowText(Marshal.C_String("20:10"), cr);

            Cairo.PatternDestroy(pattern);
            Cairo.Destroy(cr);
            Cairo.SurfaceDestroy(surface);

            request->Type = RequestType.Redraw;
            var req = (Redraw*)request;
            req->WindowID = winID;
            req->X = 0;
            req->Y = 0;
            req->Width = VBE.Xres;
            req->Height = height;
            Compositor.Server.Write(xData);
        }

        internal static void LoadIDE(bool IsPrimary, bool IsMaster)
        {
            var xIDE = new IDE(IsPrimary, IsMaster);

            bool Clean = true;
            if (xIDE.IsValid)
            {
                switch(xIDE.Device)
                {
                    case Device.IDE_ATA:
                        {
                            /*
                             * First we check If it has partitions,
                             *      If parition.count > 0
                             *          Add Individual Partitions
                             */
                            var xMBR = new MBR(xIDE);
                            if (xMBR.PartInfo.Count > 0)
                            {
                                for (int i = 0; i < xMBR.PartInfo.Count; i++)
                                {
                                    /*
                                     * Iterate over all FileSystem Drivers and check which is valid
                                     */
                                    var xFileSystem = new FatFileSystem(xMBR.PartInfo[i]);
                                    if (xFileSystem.IsValid)
                                    {
                                        VirtualFileSystem.MountDevice(null, xFileSystem);
                                        Clean = false;
                                    }
                                    else
                                        Heap.Free(xFileSystem);
                                }
                            }
                            xMBR.Clean();
                        }
                        break;
                }
            }

            if (Clean)
                Heap.Free(xIDE);
        }
    }
}
