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
    public class FatStream : Stream
    {
        private FatFileSystem mFS;

        private string mFileName;
        private uint mFileSize;

        private uint mFirstCluster;
        private uint mCurrentCluster;
        private int mPosition;
        
        private byte[] mBufferCluster;

        public FatStream(FatFileSystem aFS, string aName, uint aFirstCluster, uint aSize)
            :base(aName, aSize)
        {
            this.mFS = aFS;
            this.mFileName = aName;
            this.mFileSize = aSize;

            this.mFirstCluster = aFirstCluster;
            this.mCurrentCluster = aFirstCluster;
            this.mPosition = 0;
                        
            this.mBufferCluster = new byte[aFS.SectorsPerCluster * 512];
            LoadCluster(mFirstCluster);
        }
        
        public override int Read(byte[] aBuffer, int aCount)
        {
            int BufferPosition = 0, RelativePosition = mPosition % mBufferCluster.Length, EffectiveBytesCopied = 0;

            if (mPosition + aCount > mFileSize)
                aCount = (int)(mFileSize - mPosition);

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

        public override bool Write(byte[] aBuffer, int aCount)
        { return false; }

        public override int Position()
        { return mPosition; }

        public override bool CanSeek()
        { return true; }

        public override bool CanRead()
        { return true; }

        public override bool CanWrite()
        { return false; }

        public override bool Seek(int val, SEEK pos)
        { return false; }

        public override bool Close()
        {
            Heap.Free(mBufferCluster);
            Heap.Free(this);
            return true;
        }
    }
}
