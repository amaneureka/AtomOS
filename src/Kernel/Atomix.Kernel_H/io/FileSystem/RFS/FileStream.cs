using System;

using Atomix.Kernel_H.lib;
using Atomix.Kernel_H.core;

namespace Atomix.Kernel_H.io.FileSystem.RFS
{
    public unsafe class FileStream : Stream
    {
        private RamFile mRamFile;
        private uint mSize;
        private uint mChunkSize;
        private uint mPosition;

        private IList<uint> mChunks;

        public FileStream(RamFile aRamFile)
        {
            this.mRamFile = aRamFile;
            this.mChunkSize = aRamFile.ChunkSize;
            this.mSize = aRamFile.Size;
            this.mChunks = aRamFile.Chunks;
            this.mPosition = 0;
        }

        public override bool Read(byte[] aBuffer, uint count)
        {
            if (count > aBuffer.Length)
                return false;

            if (mPosition + count > mSize)
                return false;

            int CurrentChunk = (int)(mPosition / mChunkSize);
            int offset = (int)(mPosition % mChunkSize);
            int p = 0;
            while (count != p)
            {
                for (int i = offset; i < mChunkSize; i++)
                {
                    aBuffer[p] = ((byte*)mChunks[CurrentChunk])[i];
                    p++;
                }
                CurrentChunk++;
                offset = 0;
            }
            return true;
        }

        public override bool Write(byte[] aBuffer, uint count)
        {
            if (count > aBuffer.Length)
                return false;

            int CurrentChunk = (int)(mPosition / mChunkSize);
            int offset = (int)(mPosition % mChunkSize);
            int p = 0;
            while (count != p)
            {
                if (CurrentChunk == mChunks.Count)
                {
                    //We're at the end
                    mChunks.Add(Heap.kmalloc(mChunkSize));
                }

                for (int i = offset; i < mChunkSize; i++)
                {
                    aBuffer[p] = ((byte*)mChunks[CurrentChunk])[i];
                    p++;
                }
                CurrentChunk++;
                offset = 0;
            }
            return true;
        }

        public override bool Seek(uint val, SEEK pos)
        {
            switch (pos)
            {
                case SEEK.SEEK_FROM_CURRENT:
                    {
                        if (mPosition + val > mSize)
                            return false;
                        mPosition += val;
                    }
                    break;
                case SEEK.SEEK_FROM_ORIGIN:
                    {
                        if (val > mSize)
                            return false;
                        mPosition = val;
                    }
                    break;
                case SEEK.SEEK_FROM_END:
                    {
                        if (val > mSize)
                            return false;
                        mPosition = mSize - val;
                    }
                    break;
            }
            return true;
        }

        public override uint Position()
        { return mPosition; }

        public override bool CanSeek()
        { return true; }

        public override bool CanRead()
        { return true; }

        public override bool CanWrite()
        { return true; }
    }
}
