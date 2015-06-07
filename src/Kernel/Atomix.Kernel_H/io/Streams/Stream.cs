using System;

using Atomix.Kernel_H.io.FileSystem.VFS;

namespace Atomix.Kernel_H.io
{
    public unsafe abstract class Stream
    {
        public FileAttribute Attribute;
        public Node Entry;

        public abstract Stream CreateInstance(FileAttribute fa);

        public abstract bool Read(byte[] data, uint pos);

        public abstract byte ReadByte(uint pos);

        public abstract bool Write(byte[] data, uint pos);

        public abstract bool Write(byte data, uint pos);  
      
        public bool Close()
        {
            return ((File)Entry).Close(this);
        }
    }

    public enum FileAttribute : int
    {
        //Bit 0 : Read
        //Bit 1 : Write
        //Bit 2 : Append
        //Bit 3 : Create if not exist
        READ_ONLY = 1,
        WRITE_ONLY = 2,
        APPEND = 4,
        READ_WRITE = 3,
        READ_APPEND = 5,
        CREATE = 8,
        READ_CREATE = 9,
        WRITE_CREATE = 10,
        APPEND_CREATE = 12,
        READ_WRITE_CREATE = 11,
        READ_APPEND_CREATE = 13
    }
}
