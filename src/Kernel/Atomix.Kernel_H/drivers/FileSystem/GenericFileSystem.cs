using System;
using System.Collections.Generic;

namespace Atomix.Kernel_H.drivers.FileSystem
{
    public abstract class GenericFileSystem : Stream
    {
        protected bool mValid = false;

        public bool IsValidFileSystem
        { get { return mValid; } }

        public abstract byte[] ReadFile(string FileName);

        public abstract unsafe byte* ReadFile(string FileName, out uint Length);

        public abstract unsafe byte* ReadFile(int EntryNo);
    }
}
