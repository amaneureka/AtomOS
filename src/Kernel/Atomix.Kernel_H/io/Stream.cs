using System;

namespace Atomix.Kernel_H.io
{
    public abstract class Stream
    {
        public abstract bool CanSeek();
        public abstract int Position();
        public abstract bool CanRead();
        public abstract bool CanWrite();

        public abstract bool Write(byte[] aBuffer, int aCount);
        public abstract int Read(byte[] aBuffer, int aCount);
        public abstract bool Seek(int val, SEEK pos);

        public abstract bool Close();
    }
    public enum SEEK
    {
        SEEK_FROM_ORIGIN,
        SEEK_FROM_CURRENT,
        SEEK_FROM_END,
    }
}
