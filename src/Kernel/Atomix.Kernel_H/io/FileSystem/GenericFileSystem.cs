/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Generic File System class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomix.Kernel_H.Devices;

namespace Atomix.Kernel_H.IO.FileSystem
{
    public abstract class GenericFileSystem
    {
        internal Storage IDevice;
        protected bool mIsValid;
        protected FileSystemType mFSType;

        internal bool IsValid
        { get { return mIsValid; } }

        internal FileSystemType FileSystem
        { get { return mFSType; } }

        internal abstract Stream GetFile(string[] path, int pointer);

        internal abstract bool CreateFile(string[] path, int pointer);
    }

    public enum FileSystemType : byte
    {
        None = 0x0,
        FAT = 0x1
    }
}
