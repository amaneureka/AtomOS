using System;

using Atomix.Kernel_H.core;

namespace Atomix.Kernel_H.io.Streams
{
    public unsafe class Pipe : Stream
    {
        public readonly uint PacketSize;
        public readonly uint MaximumLimit;

        private byte* Address;

        uint* WritePosition;
        uint* ReadPointer;

        public Pipe(uint aPacketSize, uint aMaximumLimit, FileAttribute fa)
        {
            this.PacketSize = aPacketSize;
            this.MaximumLimit = aMaximumLimit;
            this.Attribute = fa;

            Address = (byte*)Heap.kmalloc(aMaximumLimit + 8);
            WritePosition = (uint*)(Address + aMaximumLimit);
            ReadPointer = (uint*)(Address + aMaximumLimit + 4);

            *WritePosition = 0;
            *ReadPointer = 0;
        }

        public Pipe(uint aPacketSize, uint aMaximumLimit, uint aAddress, uint p1, uint p2, FileAttribute fa)
        {
            this.PacketSize = aPacketSize;
            this.MaximumLimit = aMaximumLimit;
            this.Attribute = fa;

            Address = (byte*)aAddress;
            WritePosition = (uint*)p1;
            ReadPointer = (uint*)p2;
        }

        /// <summary>
        /// Send the package to Pipe, Special case if pos = 1 then it will seek read pointer of all connected pipes to zero
        /// </summary>
        /// <param name="data"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public override bool Write(byte[] data, uint pos)
        {
            if (((int)Attribute & (int)FileAttribute.WRITE_ONLY) == 0)
                return false;

            if (data.Length != PacketSize)
                return false;

            if (*WritePosition + PacketSize > MaximumLimit)
            {
                if (pos == 1)
                    *ReadPointer = 0;
                *WritePosition = 0;
            }

            for (int i = 0; i < PacketSize; i++)
                Address[*WritePosition + i] = data[i];

            *WritePosition += PacketSize;

            return true;
        }

        public override bool Write(byte data, uint pos)
        {
            return false;
        }

        public override bool Read(byte[] data, uint pos)
        {
            if (((int)Attribute & (int)FileAttribute.READ_ONLY) == 0)
                return false;
            
            if (data.Length != PacketSize)
                return false;
            
            if (*ReadPointer == *WritePosition)
                return false;
            
            if (*ReadPointer + PacketSize > MaximumLimit)
                *ReadPointer = 0;
            
            for (int i = 0; i < PacketSize; i++)
                data[i] = Address[*ReadPointer + i];

            *ReadPointer += PacketSize;

            return true;
        }

        public override byte ReadByte(uint pos)
        {
            return 0;
        }

        public override Stream CreateInstance(FileAttribute fa)
        {
            return new Pipe(
                PacketSize, 
                MaximumLimit, 
                (uint)Address, 
                (uint)WritePosition, 
                (uint)ReadPointer, fa);
        }
    }
}
