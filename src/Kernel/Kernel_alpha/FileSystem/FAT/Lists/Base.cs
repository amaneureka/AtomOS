using System;

namespace Kernel_alpha.FileSystem.FAT.Lists
{
    public abstract class Base
    {
        public readonly FileSystem FileSystem;
        public readonly string Name;

        protected Base(string aName)
        {
            //FileSystem = aFileSystem;
            Name = aName;
        }

        // Size might be updated in an ancestor destructor or on demand,
        // so its not a readonly field
        protected UInt64 mSize;
        public virtual UInt64 Size
        {
            get { return mSize; }
        }

        string xdate;

        public virtual string ModifiedDate
        {
            get { return xdate; }
            set { xdate = value; }
        }

    }
} 
