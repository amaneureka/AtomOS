using System;

namespace Atomix.Kernel_H.drivers.FileSystem.VFS
{
    public class SuperNode : Node
    {
        private readonly GenericFileSystem aData;

        public SuperNode(string aName, GenericFileSystem MountPoint)
            : base(aName)
        {
            this.aData = MountPoint;
        }

        public GenericFileSystem Open()
        {
            return aData;
        }
    }
}
