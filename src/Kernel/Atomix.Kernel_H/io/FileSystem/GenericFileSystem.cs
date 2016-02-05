using System;

using Atomix.Kernel_H.devices;

namespace Atomix.Kernel_H.io.FileSystem
{
    public abstract class GenericFileSystem
    {
        public Storage IDevice;
        protected bool mIsValid;
        protected FileSystemType mFSType;

        public bool IsValid
        { get { return mIsValid; } }

        public FileSystemType FileSystem
        { get { return mFSType; } }

        public abstract Stream GetFile(string[] path, int pointer);

        public abstract bool CreateFile(string[] path, int pointer);
    }

    public enum FileSystemType : byte
    {
        None = 0x0,
        FAT = 0x1
    }
}
