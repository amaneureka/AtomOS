/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Window Class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomix.Kernel_H.Core;
using Atomix.Kernel_H.Devices;
using Atomix.Kernel_H.Arch.x86;
using Atomix.Kernel_H.Lib.Cairo;

namespace Atomix.Kernel_H.Gui
{
    internal class Window
    {
        internal uint Buffer;
        internal string HashID;

        internal int ID;
        internal int ClientID;

        internal int X;
        internal int Y;

        internal int Width;
        internal int Height;

        internal uint Surface;

        internal Window(int aClientID, int aXpos, int aYpos, int aWidth, int aHeight)
        {
            ClientID = aClientID;
            X = aXpos;
            Y = aYpos;

            Width = aWidth;
            Height = aHeight;

            HashID = GenerateNewHashID();
            Buffer = SHM.Obtain(HashID, (uint)(aWidth * aHeight * 4), true);

            Surface = Cairo.ImageSurfaceCreateForData(aWidth * 4, aHeight, aWidth, ColorFormat.ARGB32, Buffer);
        }

        internal static string GenerateNewHashID()
        {
            // TODO: memory get more and more fragmented this way
            // come up with better idea
            string tid = Timer.TicksFromStart.ToString();
            string xid = "win." + tid;

            Heap.Free(tid);
            return xid;
        }
    }
}
