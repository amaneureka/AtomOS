/* Copyright (C) Atomix OS Development, Inc - All Rights Reserved
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* Written by SANDEEP ILIGER <sandeep.iliger@gmail.com>, 07-2014
*/

using System;
using System.Collections.Generic;
using Kernel_alpha.Drivers;
using Kernel_alpha.FileSystem.FAT;

namespace Kernel_alpha.FileSystem
{
    public abstract class GenericFileSystem
    {
        protected BlockDevice IDevice;
        protected bool mIsValid;
        protected FileSystemType mFSType;

        public BlockDevice Device
        { get { return IDevice; } }

        public bool IsValid
        { get { return mIsValid; } }

        public FileSystemType FSType
        { get { return mFSType; } }

        public abstract void ChangeDirectory(string DirName);
        public abstract void MakeDirectory(string DirName);
        public abstract byte[] ReadFile(string FileName);
        public abstract List<VFS.Entry.Base> ReadDirectory(string DirName = null);
    }

    public abstract class ACompare
    {
        public abstract bool Compare(byte[] data, uint offset, FatType type);       
    }

    public enum FileSystemType : int
    {
        None = 0,
        FAT  = 1
    };
}
