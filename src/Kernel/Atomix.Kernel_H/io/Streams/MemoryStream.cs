using System;

using Atomix.Kernel_H.core;
using Atomix.Kernel_H.io;
using Atomix.Kernel_H.arch.x86;

namespace Atomix.Kernel_H.io.Streams
{
    public unsafe class MemoryStream : Stream
    {
        protected uint Address;
        protected uint Length;
        
        public MemoryStream(byte[] objs, FileAttribute fa)
        {
            this.Attribute = fa;
            this.Address = Native.GetAddress(objs) + 0x10;
            this.Length = (uint)objs.Length;
        }

        public MemoryStream(UInt32 Start, UInt32 Size, FileAttribute fa)
        {
            this.Attribute = fa;
            this.Address = Start;
            this.Length = Size;
        }
        
        public override byte ReadByte(uint pos)
        {
            if (((int)Attribute & (int)FileAttribute.READ_ONLY) == 0)
                return 0;

            if (pos >= Length)
                return 0;

            return *((byte*)(Address + pos));
        }

        public override bool Read(byte[] data, uint pos)
        {
            if (((int)Attribute & (int)FileAttribute.READ_ONLY) == 0)
                return false;

            if (pos + data.Length > Length)
                return false;

            uint NewAdd = Address + pos;
            for (int i = 0; i < data.Length; i++)
                data[i] = *((byte*)(NewAdd + i));
            return true;
        }

        public override bool Write(byte data, uint pos)
        {
            if (((int)Attribute & (int)FileAttribute.WRITE_ONLY) == 0)
                return false;

            if (pos >= Length)
                return false;
            *((byte*)(Address + pos)) = data;
            return true;
        }

        public override bool Write(byte[] data, uint pos)
        {
            if (((int)Attribute & (int)FileAttribute.WRITE_ONLY) == 0)
                return false;

            if (pos + data.Length > Length)
                return false;

            uint NewAdd = Address + pos;
            for (int i = 0; i < data.Length; i++)
                *((byte*)(NewAdd + i)) = data[i];
            return true;
        }

        public override Stream CreateInstance(FileAttribute fa)
        {
            return new MemoryStream(Address, Length, fa);
        }

        public bool Close()
        {
            Heap.Free(Address, Length);
            return true;
        }
    }
}
