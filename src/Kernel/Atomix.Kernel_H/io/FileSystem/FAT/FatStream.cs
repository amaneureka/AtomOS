using System;

namespace Atomix.Kernel_H.io.FileSystem.FAT
{
    public class FatStream : Stream
    {
        private FatFileSystem FS;
        private string Name;
        private uint FirstCluster;
        private uint Size;

        public FatStream(FatFileSystem aFS, string aName, uint aFirstCluster, uint aSize)
        {
            this.FS = aFS;
            this.Name = aName;
            this.FirstCluster = aFirstCluster;
            this.Size = aSize;
        }
        
        public override bool Write(byte[] aBuffer, uint offset)
        {
            return false;
        }
        
        public override bool Read(byte[] aBuffer, uint count)
        {
            return false;
        }

        public override uint Position()
        { return 0; }

        public override bool CanSeek()
        { return true; }

        public override bool CanRead()
        { return true; }

        public override bool CanWrite()
        { return false; }

        public override bool Seek(uint val, SEEK pos)
        {
            return false;
        }
    }
}
