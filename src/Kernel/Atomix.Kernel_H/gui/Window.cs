/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is strictly prohibited
*                   Proprietary and confidential
* PURPOSE:          Compositor Window Class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomix.Kernel_H.core;

using Atomix.Kernel_H.devices;
using Atomix.Kernel_H.lib.crypto;

namespace Atomix.Kernel_H.gui
{
    public class Window
    {
        public readonly int ClientID;
        public readonly uint HashCode;
        public readonly string HashString;
        
		public int Width;
		public int Height;
		public uint Buffer;

		public int PositionX;

		public int PositionY;

        public Window(int aClientID)
        {
            this.ClientID = aClientID;
            this.HashCode = ("Compositor").GetHashCode(Timer.TicksFromStart);
            
			var HashCodeString = Convert.ToString(HashCode);
            HashString = "win." + HashCodeString;
            Heap.Free(HashCodeString);
            Debug.Write("[Window]: Window Created `%s`\n", HashString);
        }
    }
}
