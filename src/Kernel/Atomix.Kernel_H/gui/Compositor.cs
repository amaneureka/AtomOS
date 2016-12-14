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
using Atomix.Kernel_H.Arch.x86;
using Atomix.Kernel_H.Drivers.Video;

namespace Atomix.Kernel_H.Gui
{
    internal static class Compositor
    {
        internal const int PACKET_SIZE = 48;

        static Bitmap MainBuffer;
        static Bitmap BackBuffer;

        static IList<Pipe> Clients;
        static IList<Window> Windows;

        internal static Pipe Server;

        internal unsafe static void Setup(Process aParent)
        {
            Server = new Pipe(PACKET_SIZE, 1000);
            MainBuffer = new Bitmap(VBE.VirtualFrameBuffer, VBE.Xres, VBE.Yres, VBE.BytesPerPixel);
            BackBuffer = new Bitmap(VBE.SecondaryBuffer, VBE.Xres, VBE.Yres, VBE.BytesPerPixel);

            Clients = new IList<Pipe>();
            Windows = new IList<Window>();

            new Thread(aParent, Renderer, Heap.kmalloc(0x1000) + 0x1000, 0x1000).Start();
            new Thread(aParent, HandleRequest, Heap.kmalloc(0x1000) + 0x1000, 0x1000).Start();
        }

        internal static int CreateConnection(Pipe aClient)
        {
            if (aClient.PacketSize != PACKET_SIZE) return -1;

            Clients.Add(aClient);
            return (Clients.Count - 1);
        }

        private static void Renderer()
        {
            Bitmap canvas, screen;
            while (true)
            {
                canvas = BackBuffer;
                screen = MainBuffer;

                Monitor.AcquireLock(Windows);
                int count = Windows.Count;
                for (int i = 0; i < count; i++)
                {
                    var win = Windows[i];
                    canvas.Draw(win.Buffer, win.X, win.Y);
                }
                Monitor.ReleaseLock(Windows);

                screen.Draw(canvas, 0, 0);
            }
        }

        private static unsafe void HandleRequest()
        {
            var packet = new byte[PACKET_SIZE];

            while(true)
            {
                Server.Read(packet);

                var request = (GuiRequest*)Native.GetContentAddress(packet);
                Debug.Write("New Packet: %d\n", request->HashID);
                // TODO: verify client ID and HashID
                if (request->ClientID == -1) continue;

                switch(request->Type)
                {
                    case RequestType.NewWindow:
                        {
                            var winRequest = (NewWindow*)request;

                            if (winRequest->Width == 0 || winRequest->Height == 0)
                            {
                                winRequest->Error = ErrorType.InvalidParameters;
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

                            Marshal.Copy(winRequest->Hash, newWindow.HashID, newWindow.HashID.Length);

                            Monitor.AcquireLock(Windows);
                            Windows.Add(newWindow);
                            Monitor.ReleaseLock(Windows);
                        }
                        break;
                    default:
                        // Invalid
                        break;
                }

                Clients[request->ClientID].Write(packet, false);
            }
        }
    }
}
