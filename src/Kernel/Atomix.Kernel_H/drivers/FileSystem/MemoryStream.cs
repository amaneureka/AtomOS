using System;
using System.Collections.Generic;

namespace Atomix.Kernel_H.drivers.FileSystem
{
    public unsafe class MemoryStream : Stream
    {
        public readonly byte* Address8;
        public readonly UInt32* Address32;

        public MemoryStream(UInt32 Start, UInt32 Size)
            : base(Size)
        {
            this.Address8 = (byte*)Start;
            this.Address32 = (UInt32*)Start;
        }
    }
}
