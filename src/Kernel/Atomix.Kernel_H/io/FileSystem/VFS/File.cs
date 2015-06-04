using System;

namespace Atomix.Kernel_H.io.FileSystem.VFS
{
    public class File : Node
    {
        private Stream Data;

        public File(string aName, Stream MountPoint)
            :base(aName)
        {
            this.Data = MountPoint;
        }

        public Stream Open(FileAttribute fa)
        {
            Data.Attribute = fa;
            return Data;
        }
    }
}
