using System;

using Atomix.Kernel_H.lib;
using Atomix.Kernel_H.core;

namespace Atomix.Kernel_H.io.FileSystem.RFS
{
    public unsafe class FileStream : Stream
    {
        public readonly RamFile RamFile;
        private int mPosition;
        
        public FileStream(RamFile aRamFile)
            :base(aRamFile.Name, aRamFile.Length)
        {
            this.RamFile = aRamFile;
            this.mPosition = 0;
        }

        public override int Read(byte[] aBuffer, int aCount)
        {
            if (aCount + mPosition > RamFile.Length)
                aCount = (int)RamFile.Length - mPosition;

            var xAddress = (byte*)(RamFile.StartAddress + mPosition);
            for (int index = 0; index < aCount; index++)
                aBuffer[index] = xAddress[index];

            mPosition += aCount;
            return aCount;
        }

        public override bool Write(byte[] aBuffer, int aCount)
        {
            return false;
        }

        public override bool CanRead()
        { return true; }

        public override bool CanSeek()
        { return false; }

        public override bool CanWrite()
        { return false; }

        public override int Position()
        { return mPosition; }

        public override bool Seek(int val, SEEK pos)
        { return false; }

        public override bool Close()
        {
            Heap.Free(this);
            return true;
        }
    }
}
