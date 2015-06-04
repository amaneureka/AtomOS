using System;

using Atomix.Kernel_H.io;

namespace Atomix.Kernel_H.io.Streams
{
    public unsafe class MemoryStream : Stream
    {
        protected uint Address;
        protected uint Length;
        
        public MemoryStream(UInt32 Start, UInt32 Size)
        {
            this.Address = Start;
            this.Length = Size;
        }
        
        public override byte ReadByte(uint pos)
        {
            if (pos >= Length)
                return 0;
            return *((byte*)(Address + pos));
        }

        public override bool Read(byte[] data, uint pos)
        {
            if (pos + data.Length > Length)
                return false;

            uint NewAdd = Address + pos;
            for (int i = 0; i < data.Length; i++)
                data[i] = *((byte*)(NewAdd + i));
            return true;
        }

        public override bool Write(byte data, uint pos)
        {
            if (pos >= Length)
                return false;
            *((byte*)(Address + pos)) = data;
            return true;
        }

        public override bool Write(byte[] data, uint pos)
        {
            if (pos + data.Length > Length)
                return false;

            uint NewAdd = Address + pos;
            for (int i = 0; i < data.Length; i++)
                *((byte*)(NewAdd + i)) = data[i];
            return true;
        }
    }
}
