using System;
using System.Collections.Generic;

namespace Kernel_alpha.FileSystem.FAT.Lists
{
    class File : Base
    {
        public File( string aName, UInt64 aSize, string adate)
            : base(aName)
        {
            mSize = aSize;
            ModifiedDate = adate;
        }
    }
}
