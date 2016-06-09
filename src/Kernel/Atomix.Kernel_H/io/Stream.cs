/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is strictly prohibited
*                   Proprietary and confidential
* PURPOSE:          Stream Abstract Class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomix.Kernel_H.core;

namespace Atomix.Kernel_H.io
{
    public abstract class Stream
    {
        public readonly string FileName;
        public readonly uint FileSize;

        public Stream(string aFileName, uint aSize)
        {
            FileName = aFileName;
            FileSize = aSize;
        }

        public abstract bool CanSeek();
        public abstract int Position();
        public abstract bool CanRead();
        public abstract bool CanWrite();

        public abstract bool Write(byte[] aBuffer, int aCount);
        public abstract int Read(byte[] aBuffer, int aCount);
        public abstract bool Seek(int val, SEEK pos);

        public abstract bool Close();

        public string ReadToEnd()
        {
            var xData = new byte[FileSize];
            Read(xData, xData.Length);
            var xResult = lib.encoding.ASCII.GetString(xData, 0, xData.Length);
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
