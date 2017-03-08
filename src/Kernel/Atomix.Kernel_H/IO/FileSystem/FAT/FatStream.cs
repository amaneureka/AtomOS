/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          FAT Stream Class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomixilc.Lib;

using Atomix.Kernel_H.Core;

namespace Atomix.Kernel_H.IO.FileSystem.FAT
{
    internal unsafe class FatStream : Stream
    {
        private FatFileSystem mFS;

        private string mFileName;
        private int mFileSize;

        private uint mFirstCluster;
        private uint mCurrentCluster;
        private int mPosition;

        private byte* mBufferCluster;
        private int mBufferLength;

        internal FatStream(FatFileSystem aFS, string aName, uint aFirstCluster, int aSize)
            :base(aName, aSize)
        {
            mFS = aFS;
            mFileName = aName;
            mFileSize = aSize;

            mFirstCluster = aFirstCluster;
            mCurrentCluster = aFirstCluster;
            mPosition = 0;

            mBufferLength = (int)(aFS.SectorsPerCluster * 512);
            mBufferCluster = (byte*)Heap.kmalloc(aFS.SectorsPerCluster * 512);
            LoadCluster(mFirstCluster);
        }

        internal override int Read(byte[] aBuffer, int aCount)
        {
            if (aCount > aBuffer.Length)
                aCount = aBuffer.Length;

            return Read((byte*)aBuffer.GetDataOffset(), aCount);
        }

        internal override int Read(byte* aBuffer, int aCount)
        {
            int BufferPosition = 0, RelativePosition = mPosition % mBufferLength, EffectiveBytesCopied = 0;

            if (mPosition + aCount > mFileSize)
                aCount = mFileSize - mPosition;

            do
            {
                int LengthToCopy = mBufferLength - RelativePosition;

                if (LengthToCopy > aCount)
                    LengthToCopy = aCount;

                Memory.FastCopy((uint)aBuffer + (uint)BufferPosition, (uint)mBufferCluster + (uint)RelativePosition, (uint)LengthToCopy);

                aCount -= LengthToCopy;
                mPosition += LengthToCopy;
                BufferPosition += LengthToCopy;
                RelativePosition += LengthToCopy;
                EffectiveBytesCopied += LengthToCopy;

                if (RelativePosition >= mBufferLength)
                {
                    RelativePosition = 0;
                    if (ReadNextCluster() == false)
                        return EffectiveBytesCopied;
                }
            }
            while (aCount > 0);
            return EffectiveBytesCopied;
        }

        private bool ReadNextCluster()
        {
            uint xNextCluster = mFS.GetClusterEntryValue(mCurrentCluster);

            if (FatFileSystem.IsClusterLast(xNextCluster))
                return false;

            if (FatFileSystem.IsClusterFree(xNextCluster)
                || FatFileSystem.IsClusterBad(xNextCluster)
                || FatFileSystem.IsClusterReserved(xNextCluster))
                return false;

            LoadCluster(xNextCluster);

            mCurrentCluster = xNextCluster;
            return true;
        }

        private bool LoadCluster(uint Cluster)
        {
            UInt32 xSector = mFS.DataSector + ((Cluster - mFS.RootCluster) * mFS.SectorsPerCluster);
            return mFS.IDevice.Read(xSector, mFS.SectorsPerCluster, (byte*)mBufferCluster);
        }

        internal override int Write(byte[] aBuffer, int aCount)
        { return 0; }

        internal override int Write(byte* aBuffer, int aCount)
        { return 0; }

        internal override int Position()
        { return mPosition; }

        internal override bool CanSeek()
        { return true; }

        internal override bool CanRead()
        { return true; }

        internal override bool CanWrite()
        { return false; }

        internal override int Seek(int val, SEEK pos)
        { return 0; }

        internal override bool Close()
        {
            Heap.Free((uint)mBufferCluster, (uint)mBufferLength);
            Heap.Free(this);
            return true;
        }
    }
}
