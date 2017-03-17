/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Stream Abstract Class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomix.Kernel_H.IO
{
    internal abstract class Stream
    {
        internal abstract bool Seek(int aValue, FileSeek aSeek);
        internal abstract int Write(byte[] aBuffer, int aCount);
        internal abstract int Write(byte[] aBuffer, int aOffset, int aCount);
        internal abstract int Read(byte[] aBuffer, int aCount);
        internal abstract int Read(byte[] aBuffer, int aOffset, int aCount);
        internal abstract bool Close();
    }
}
