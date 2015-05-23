using System;
using System.Collections.Generic;

namespace Atomix.Kernel_H.drivers.FileSystem
{
    public abstract class GenericFileSystem
    {
        protected bool mValid;
        public readonly Stream Stream;

        public bool IsValidFileSystem
        { get { return mValid; } }

        public GenericFileSystem(Stream aStream)
        {
            this.Stream = aStream;
        }

        public static GenericFileSystem Detect(Stream stream)
        {
            GenericFileSystem FS = null;
            //Check if this file system driver is present or not
            return FS;
        }
        
        public abstract byte[] ReadFile(string FileName);

        public abstract unsafe byte* ReadFile(string FileName, out uint Length);

        public abstract unsafe byte* ReadFile(int EntryNo);
    }
}
