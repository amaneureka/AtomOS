/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          FAT Stream Class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomix.Kernel_H.Core;

namespace Atomix.Kernel_H.IO.FileSystem.FAT
{
    internal class FatStream : Stream
    {
        private FatFileSystem mFS;

        private string mFileName;
        private int mFileSize;

        private uint mFirstCluster;
        private uint mCurrentCluster;
        private int mPosition;

        private byte[] mBufferCluster;

        internal FatStream(FatFileSystem aFS, string aName, uint aFirstCluster, int aSize)
            :base(aName, aSize)
        {
            mFS = aFS;
            mFileName = aName;
            mFileSize = aSize;

            mFirstCluster = aFirstCluster;
            mCurrentCluster = aFirstCluster;
            mPosition = 0;

            mBufferCluster = new byte[aFS.SectorsPerCluster * 512];
            LoadCluster(mFirstCluster);
        }

        internal override int Read(byte[] aBuffer, int aCount)
        {
            int BufferPosition = 0, RelativePosition = mPosition % mBufferCluster.Length, EffectiveBytesCopied = 0;

            if (mPosition + aCount > mFileSize)
                aCount = mFileSize - mPosition;

            do
            {
                int LengthToCopy = mBufferCluster.Length - RelativePosition;

                if (LengthToCopy > aCount)
                    LengthToCopy = aCount;

                Array.Copy(mBufferCluster, RelativePosition, aBuffer, BufferPosition, LengthToCopy);

                aCount -= LengthToCopy;
                mPosition += LengthToCopy;
                BufferPosition += LengthToCopy;
                RelativePosition += LengthToCopy;
                EffectiveBytesCopied += LengthToCopy;

                if (RelativePosition >= mBufferCluster.Length)
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
            return mFS.IDevice.Read(xSector, mFS.SectorsPerCluster, mBufferCluster);
        }

        internal override bool Write(byte[] aBuffer, int aCount)
        { return false; }

        internal override int Position()
        { return mPosition; }

        internal override bool CanSeek()
        { return true; }

        internal override bool CanRead()
        { return true; }

        internal override bool CanWrite()
        { return false; }

        internal override bool Seek(int val, SEEK pos)
        { return false; }

        internal override bool Close()
        {
            Heap.Free(mBufferCluster);
            Heap.Free(this);
            return true;
        }
    }
}
