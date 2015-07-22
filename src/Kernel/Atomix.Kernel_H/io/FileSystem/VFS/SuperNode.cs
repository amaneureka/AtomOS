using System;

namespace Atomix.Kernel_H.io.FileSystem.VFS
{
    public class SuperNode : Node
    {
        protected GenericFileSystem ParentDevice;

        public SuperNode(string aName, GenericFileSystem FileSys)
            :base(aName)
        {
            this.ParentDevice = FileSys;
        }

        public GenericFileSystem GetFS
        { get { return ParentDevice; } }
    }
}
