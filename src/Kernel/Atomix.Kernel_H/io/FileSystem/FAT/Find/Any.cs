using System;
using System.Collections.Generic;

namespace Atomix.Kernel_H.io.FileSystem.FAT.Find
{
    public class Any : Comparison
    {
        public Any() { }

        public override bool Compare(byte[] data, int offset, FatType type)
        {
            switch ((FileNameAttribute)data[offset + (uint)Entry.DOSName])
            {
                case FileNameAttribute.LastEntry:
                case FileNameAttribute.Deleted:
                case FileNameAttribute.Escape:
                case FileNameAttribute.Dot:
                    return false;
                default:
                    return true;
            }
        }
    }
}
