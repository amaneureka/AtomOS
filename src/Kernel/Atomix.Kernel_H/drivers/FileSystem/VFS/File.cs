using System;
using System.Collections.Generic;

namespace Atomix.Kernel_H.drivers.FileSystem.VFS
{
    public class File : Node
    {
        protected Stream Data;

        public File(string aName, Stream MountPoint)
            :base(aName)
        {
            this.Data = MountPoint;
        }
    }
}
