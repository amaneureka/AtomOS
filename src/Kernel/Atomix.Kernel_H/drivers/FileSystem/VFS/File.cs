using System;

namespace Atomix.Kernel_H.drivers.FileSystem.VFS
{
    public class File : Node
    {
        private Stream Data;
        private bool ResourceLock;

        public File(string aName, Stream MountPoint)
            :base(aName)
        {
            this.Data = MountPoint;
            this.ResourceLock = false;
        }

        public Stream Open(bool Lock)
        {
            if (ResourceLock)
                return null;

            this.ResourceLock = Lock;
            return Data;
        }

        public void Close()
        {
            this.ResourceLock = false;            
        }
    }
}
