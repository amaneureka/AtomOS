/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Memory Stream Class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomixilc.Lib;

namespace Atomix.Kernel_H.IO
{
    internal unsafe class MemoryStream : Stream
    {
        uint mAddress;
        int mLength;

        int mPointer;

        internal uint Address
        { get { return mAddress; } }

        internal int Length
        { get { return mLength; } }

        internal MemoryStream(uint aAddress, int aLength)
        {
            mAddress = aAddress;
            mLength = aLength;
            mPointer = 0;
        }

        internal override int Read(byte[] aBuffer, int aCount)
        {
            int aOffset = mPointer;
            if (aOffset + aCount >= mLength)
                aCount = mLength - aOffset;
            Memory.FastCopy(aBuffer.GetDataOffset(), mAddress, aCount);
            mPointer = aOffset + aCount;
            return aCount;
        }

        internal override int Read(byte[] aBuffer, int aOffset, int aCount)
        {
            if (aOffset + aCount >= mLength)
                aCount = mLength - aOffset;
            Memory.FastCopy(aBuffer.GetDataOffset(), mAddress, aCount);
            return aCount;
        }

        internal override int Write(byte[] aBuffer, int aCount)
        {
            int aOffset = mPointer;
            if (aOffset + aCount >= mLength)
                aCount = mLength - aOffset;
            Memory.FastCopy(mAddress, aBuffer.GetDataOffset(), aCount);
            mPointer = aOffset + aCount;
            return aCount;
        }

        internal override int Write(byte[] aBuffer, int aOffset, int aCount)
        {
            if (aOffset + aCount >= mLength)
                aCount = mLength - aOffset;
            Memory.FastCopy(mAddress, aBuffer.GetDataOffset(), aCount);
            return aCount;
        }

        internal override int Seek(int aValue, FileSeek aSeek)
        {
            switch (aSeek)
            {
                case FileSeek.Origin:
                    {
                        mPointer = 0;
                    }
                    break;
                case FileSeek.Current:
                    {
                        if (mPointer + aValue > mLength)
                            break;
                        mPointer += aValue;
                    }
                    break;
                case FileSeek.End:
                    {
                        if (aValue > mLength)
                            break;
                        mPointer = mLength - aValue;
                    }
                    break;
            }
            return mPointer;
        }

        internal override bool Close()
        {
            return true;
        }
    }
}
