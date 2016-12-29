/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Pipe Class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomixilc.Lib;

using Atomix.Kernel_H.Core;

namespace Atomix.Kernel_H.IO
{
    internal unsafe class Pipe
    {
        uint Buffer;
        uint BufferSize;
        bool[] BufferStatus;

        int ReadingPointer;
        int WritingPointer;

        internal readonly int PacketSize;
        internal readonly int PacketsCount;

        internal Pipe(int aPacketSize, int aPacketsCount)
        {
            PacketsCount = aPacketsCount;
            PacketSize = aPacketSize;
            BufferSize = (uint)(PacketsCount * PacketSize);
            Buffer = Heap.kmalloc(BufferSize);
            BufferStatus = new bool[PacketsCount];

            ReadingPointer = WritingPointer = 0;
        }

        internal bool Write(byte[] aData, bool Hangup = true)
        {
            if (aData.Length != PacketSize)
                return false;

            while (Hangup && BufferStatus[WritingPointer]) ;

            if (BufferStatus[WritingPointer])
                return false;

            Memory.FastCopy(Buffer + (uint)(WritingPointer * PacketSize), aData.GetDataOffset(), (uint)PacketSize);
            BufferStatus[WritingPointer] = true;

            WritingPointer = (WritingPointer + 1) % PacketsCount;
            return true;
        }

        internal bool Read(byte[] aData)
        {
            if (aData.Length != PacketSize)
                return false;

            while (!BufferStatus[ReadingPointer]) ;

            Memory.FastCopy(aData.GetDataOffset(), Buffer + (uint)(ReadingPointer * PacketSize), (uint)PacketSize);
            BufferStatus[ReadingPointer] = false;

            ReadingPointer = (ReadingPointer + 1) % PacketsCount;
            return true;
        }

        internal void Close()
        {
            Heap.Free(Buffer, BufferSize);
            Heap.Free(BufferStatus);
            Heap.Free(this);
        }
    }
}