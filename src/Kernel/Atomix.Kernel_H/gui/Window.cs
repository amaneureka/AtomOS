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
        
        public uint Buffer { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public int PositionX { get; set; }

        public int PositionY { get; set; }

        public Window(int aClientID)
        {
            this.ClientID = aClientID;
            this.HashCode = ("Compositor").GetsdbmHash(Timer.TicksFromStart);
            
            var HashCodeString = HashCode.ToString();
            HashString = "win." + HashCodeString;
            Heap.Free(HashCodeString);
            Debug.Write("[Window]: Window Created `%s`\n", HashString);
        }
    }
}
