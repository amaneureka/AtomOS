using System;
using System.Collections.Generic;

namespace Kernel_alpha.FileSystem.VFS.Entry
{
    public class File : Base
    {
        protected ulong Size;

        public File(string xName, ulong aSize)
            :base (xName)
        {
            this.Size = aSize;
        }
    }
}
