/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Stream Abstract Class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomix.Kernel_H.Core;

namespace Atomix.Kernel_H.IO
{
    public abstract class Stream
    {
        internal readonly string FileName;
        internal readonly int FileSize;

        internal Stream(string aFileName, int aSize)
        {
            FileName = aFileName;
            FileSize = aSize;
        }

        internal abstract bool CanSeek();
        internal abstract int Position();
        internal abstract bool CanRead();
        internal abstract bool CanWrite();

        internal abstract unsafe int Write(byte* aBuffer, int aCount);
        internal abstract unsafe int Read(byte* aBuffer, int aCount);

        internal abstract int Write(byte[] aBuffer, int aCount);
        internal abstract int Read(byte[] aBuffer, int aCount);
        internal abstract int Seek(int val, SEEK pos);

        internal abstract bool Close();
    }

    public enum SEEK : int
    {
        SEEK_FROM_ORIGIN = 0,
        SEEK_FROM_CURRENT = 1,
        SEEK_FROM_END = 2,
    }
}
