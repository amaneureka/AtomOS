using System;
using System.Collections.Generic;

namespace Atomix.Kernel_H.io.FileSystem.FAT.Find
{
    public class Empty : Comparison
    {
        public Empty() { }

        public override bool Compare(byte[] data, int offset, FatType type)
        {
            if ((FileNameAttribute)data[offset + (uint)Entry.DOSName] == FileNameAttribute.LastEntry)
                return true;

            return false;
        }
    }
}
