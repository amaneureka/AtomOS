/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          RFS (Ram File System) File Stream class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomixilc.Lib;

using Atomix.Kernel_H.Core;

namespace Atomix.Kernel_H.IO.FileSystem.RFS
{
    internal unsafe class FileStream : Stream
    {
        internal readonly RamFile RamFile;
        int mPosition;

        internal FileStream(RamFile aRamFile)
            :base(aRamFile.Name, aRamFile.Length)
        {
            RamFile = aRamFile;
            mPosition = 0;
        }

        internal override int Read(byte[] aBuffer, int aCount)
        {
            if (aBuffer.Length > aCount)
                aCount = aBuffer.Length;

            return Read((byte*)aBuffer.GetDataOffset(), aCount);
        }

        internal override unsafe int Read(byte* aBuffer, int aCount)
        {
            if (aCount + mPosition > RamFile.Length)
                aCount = RamFile.Length - mPosition;

            Memory.FastCopy((uint)aBuffer, RamFile.StartAddress + (uint)mPosition, (uint)aCount);

            mPosition += aCount;
            return aCount;
        }

        internal override int Write(byte[] aBuffer, int aCount)
        {
            return 0;
        }

        internal override unsafe int Write(byte* aBuffer, int aCount)
        {
            return 0;
        }

        internal override bool CanRead()
        { return true; }

        internal override bool CanSeek()
        { return false; }

        internal override bool CanWrite()
        { return false; }

        internal override int Position()
        { return mPosition; }

        internal override int Seek(int val, SEEK pos)
        {
            switch(pos)
            {
                case SEEK.SEEK_FROM_CURRENT:
                    mPosition += val;
                    break;
                case SEEK.SEEK_FROM_ORIGIN:
                    mPosition += val;
                    break;
                case SEEK.SEEK_FROM_END:
                    mPosition = RamFile.Length + val;
                    break;
            }

            mPosition %= RamFile.Length;
            return mPosition;
        }

        internal override bool Close()
        {
            Heap.Free(this);
            return true;
        }
    }
}
