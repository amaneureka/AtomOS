using System;

using Atomix.Kernel_H.core;

namespace Atomix.Kernel_H.io.FileSystem.VFS
{
    public class File : Node
    {
        private Stream Data;
        private uint RefCount;
        private bool Locked;

        public File(string aName, Stream MountPoint)
            :base(aName)
        {            
            this.Data = MountPoint;
            this.RefCount = 0;
            this.Data.Entry = this;
            this.Locked = false;
        }

        public Stream Open(FileAttribute fa)
        {
            if (((int)fa & (int)FileAttribute.WRITE_ONLY) != 0 ||
                ((int)fa & (int)FileAttribute.APPEND) != 0)
            {
                if (Locked)
                    return null;
                else
                    Locked = true;
            }

            RefCount++;
            if (RefCount == 1)
                return Data;//Return our base

            return Data.CreateInstance(fa);
        }

        public bool Close(Stream str)
        {
            RefCount--;
            var fa = str.Attribute;
            if (((int)fa & (int)FileAttribute.WRITE_ONLY) != 0 ||
                ((int)fa & (int)FileAttribute.APPEND) != 0)
            {
                Locked = false;
            }
            Heap.Free(str);
            return true;
        }
    }
}
