/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Compositor Window Class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomix.Kernel_H.core;

using Atomix.Kernel_H.devices;
using Atomix.Kernel_H.lib.crypto;

namespace Atomix.Kernel_H.gui
{
    internal class Window
    {
        internal readonly int ClientID;
        internal readonly uint HashCode;
        internal readonly string HashString;

        internal int Width;
        internal int Height;
        internal uint Buffer;

        internal int PositionX;

        internal int PositionY;

        public Window(int aClientID)
        {
            ClientID = aClientID;
            HashCode = ("Compositor").GetHashCode(Timer.TicksFromStart);
            
			var HashCodeString = Convert.ToString(HashCode);
            HashString = "win." + HashCodeString;
            Heap.Free(HashCodeString);
            Debug.Write("[Window]: Window Created `%s`\n", HashString);
        }
    }
}
