using System;

namespace Atomix.Kernel_H.io
{
    public abstract class Stream
    {
        public abstract bool CanSeek();
        public abstract uint Position();
        public abstract bool CanRead();
        public abstract bool CanWrite();

        public abstract bool Write(byte[] aBuffer, uint count);
        public abstract bool Read(byte[] aBuffer, uint count);
        public abstract bool Seek(uint val, SEEK pos);
    }
    public enum SEEK
    {
        SEEK_FROM_ORIGIN,
        SEEK_FROM_CURRENT,
        SEEK_FROM_END,
    }
}
