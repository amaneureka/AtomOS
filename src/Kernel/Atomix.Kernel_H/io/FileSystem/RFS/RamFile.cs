using System;

using Atomix.Kernel_H.core;

using Atomix.Kernel_H.lib;

namespace Atomix.Kernel_H.io.FileSystem.RFS
{
    public class RamFile
    {
        private string mName;
        private uint mFirstChunk;
        private uint mChunkSize;
        private uint mSize;

        public readonly IList<uint> Chunks;

        public string Name
        { get { return mName; } }

        public uint FirstChunk
        { get { return mFirstChunk; } }

        public uint ChunkSize
        { get { return mChunkSize; } }

        public uint Size
        {
            get { return mSize; }
            set { mSize = value; }
        }

        public RamFile(string aName, uint aChunkSize = 0x1000)
        {
            this.mName = aName;
            this.mChunkSize = aChunkSize;
            this.mFirstChunk = Heap.kmalloc(mChunkSize);
            this.mSize = 0;
            this.Chunks = new IList<uint>();
        }
    }
}
