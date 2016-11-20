/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Atom GUI Compositor Class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomix.Kernel_H.IO;
using Atomix.Kernel_H.Lib;
using Atomix.Kernel_H.Core;
using Atomix.Kernel_H.Arch.x86;
using Atomix.Kernel_H.Drivers.Video;

namespace Atomix.Kernel_H.Gui
{
    internal static class Compositor
    {
        internal const uint PACKET_SIZE = 32;

        static Bitmap MainBuffer;
        static Bitmap BackBuffer;

        static IList<Window> Windows;

        static Pipe Server;

        internal unsafe static void Setup(Process aParent)
        {
            Server = new Pipe(PACKET_SIZE, 100);
            MainBuffer = new Bitmap(VBE.VirtualFrameBuffer, VBE.Xres, VBE.Yres, VBE.BytesPerPixel);
            BackBuffer = new Bitmap(VBE.SecondaryBuffer, VBE.Xres, VBE.Yres, VBE.BytesPerPixel);

            Windows = new IList<Window>();

            new Thread(aParent, Renderer, Heap.kmalloc(0x1000) + 0x1000, 0x1000).Start();
            new Thread(aParent, HandleRequest, Heap.kmalloc(0x1000) + 0x1000, 0x1000).Start();
        }

        private static void Renderer()
        {
            while (true) ;
        }

        private static unsafe void HandleRequest()
        {
            var packet = new byte[PACKET_SIZE];

            while(true)
            {
                Server.Read(packet);

                var request = (GuiRequest*)Native.GetContentAddress(packet);

                // TODO: verify client ID and HashID

                switch(request->Type)
                {
                    case RequestType.NewWindow:
                        {
                            var winRequest = (NewWindow*)request;
                            var newWindow = new Window
                                (
                                    request->ClientID,
                                    winRequest->X,
                                    winRequest->Y,
                                    winRequest->Width,
                                    winRequest->Height
                                );
                            Marshal.Copy(newWindow.HashID, winRequest->Hash, (uint)newWindow.HashID.Length);
                        }
                        break;
                    default:
                        // Invalid
                        break;
                }
            }
        }
    }
}
