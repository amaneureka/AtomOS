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

        public override int Read(byte[] aBuffer, int aCount)
        {
            if (aCount + mPosition > RamFile.Length)
                aCount = (int)RamFile.Length - mPosition;

            var xAddress = (byte*)(RamFile.StartAddress + (uint)mPosition);
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
