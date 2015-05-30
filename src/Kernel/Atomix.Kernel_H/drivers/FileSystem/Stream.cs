using System;

namespace Atomix.Kernel_H.drivers.FileSystem
{
    public unsafe abstract class Stream
    {
        protected UInt32 mSize;

        public UInt32 Size
        { get { return mSize; } }

        public Stream(uint size)
        {
            this.mSize = size;
        }

        public virtual bool Read(byte[] data, int pos) 
        {
            return false;
        }

        public virtual bool Read(byte* data, int pos, int length)
        {
            return false;
        }

        public virtual bool Write(byte[] data, int pos)
        {
            return false;
        }

        public virtual bool Write(byte* data, int pos, int length)
        {
            return false;
        }
    }
}
