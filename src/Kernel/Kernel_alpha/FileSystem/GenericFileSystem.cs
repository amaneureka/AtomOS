using System;
using System.Collections.Generic;
using Kernel_alpha.Drivers;

namespace Kernel_alpha.FileSystem
{
    public abstract class GenericFileSystem
    {
        protected BlockDevice IDevice;
        protected bool mIsValid;

        public BlockDevice Device
        { get { return IDevice; } }

        public bool IsValid
        { get { return mIsValid; } }
    }
}
