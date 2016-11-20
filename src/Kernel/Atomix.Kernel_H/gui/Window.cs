/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Window Class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomix.Kernel_H.Core;
using Atomix.Kernel_H.Devices;
using Atomix.Kernel_H.Arch.x86;

namespace Atomix.Kernel_H.Gui
{
    internal class Window
    {
        Bitmap mBuffer;
        string mHashID;

        internal uint ClientID;

        internal uint X;
        internal uint Y;
        internal uint Z;

        internal Bitmap Buffer
        { get { return mBuffer; } }

        internal string HashID
        { get { return mHashID; } }

        internal Window(uint aClientID, uint aXpos, uint aYpos, uint aWidth, uint aHeight)
        {
            ClientID = aClientID;
            X = aXpos;
            Y = aYpos;
            Z = 0;

            mHashID = GenerateNewHashID();
            mBuffer = new Bitmap(mHashID, aWidth, aHeight);
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
