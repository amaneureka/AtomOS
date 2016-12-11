/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          RFS (Ram File System) File Stream class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

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
            if (aCount + mPosition > RamFile.Length)
                aCount = RamFile.Length - mPosition;

            var xAddress = (byte*)(RamFile.StartAddress + (uint)mPosition);
            for (int index = 0; index < aCount; index++)
                aBuffer[index] = xAddress[index];

            mPosition += aCount;
            return aCount;
        }

        internal override bool Write(byte[] aBuffer, int aCount)
        {
            return false;
        }

        internal override bool CanRead()
        { return true; }

        internal override bool CanSeek()
        { return false; }

        internal override bool CanWrite()
        { return false; }

        internal override int Position()
        { return mPosition; }

        internal override bool Seek(int val, SEEK pos)
        { return false; }

        internal override bool Close()
        {
            Heap.Free(this);
            return true;
        }
    }
}
