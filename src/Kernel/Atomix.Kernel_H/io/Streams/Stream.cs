using System;

namespace Atomix.Kernel_H.io
{
    public unsafe abstract class Stream
    {
        public FileAttribute Attribute;

        public abstract bool Read(byte[] data, uint pos);

        public abstract byte ReadByte(uint pos);

        public abstract bool Write(byte[] data, uint pos);

        public abstract bool Write(byte data, uint pos);        
    }

    public enum FileAttribute : int
    {
        READ_ONLY = 1,
        WRITE_ONLY = 2,
        READ_WRITE = 4,
        WRITE_APPEND = 8,
        READ_WRITE_APPEND = 16
    }
}
