using System;
using System.Collections.Generic;

namespace Kernel_alpha.FileSystem.VFS.Entry
{
    public abstract class Base
    {
        protected string aName;

        public string Name
        { get { return aName; } }

        public Base (string xName)
        {
            this.aName = xName;
        }
    }
}
