/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
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

        internal abstract bool Write(byte[] aBuffer, int aCount);
        internal abstract int Read(byte[] aBuffer, int aCount);
        internal abstract bool Seek(int val, SEEK pos);

        internal abstract bool Close();

        internal string ReadToEnd()
        {
            var xData = new byte[FileSize];
            Read(xData, xData.Length);
            var xResult = Lib.encoding.ASCII.GetString(xData, 0, xData.Length);
            Heap.Free(xData);
            return xResult;
        }
    }
    public enum SEEK
    {
        SEEK_FROM_ORIGIN,
        SEEK_FROM_CURRENT,
        SEEK_FROM_END,
    }
}
