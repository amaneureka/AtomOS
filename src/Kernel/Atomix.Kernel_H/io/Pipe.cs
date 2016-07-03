/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Pipe Class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomix.Kernel_H.Core;

namespace Atomix.Kernel_H.IO
{
    internal unsafe class Pipe
    {
        private uint Buffer;
        private uint BufferSize;
        private uint PacketSize;
        private uint PacketsCount;
        private bool[] BufferStatus;

        uint ReadingPointer;
        uint WritingPointer;

        internal Pipe(uint aPacketSize, uint aPacketsCount)
        {
            PacketsCount = aPacketsCount;
            PacketSize = aPacketSize;
            BufferSize = PacketsCount * PacketSize;
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

            var xAdd = (byte*)(Buffer + WritingPointer * PacketSize);
            for (int i = 0; i < PacketSize; i++)
                xAdd[i] = aData[i];
            BufferStatus[WritingPointer] = true;

            WritingPointer++;
            if (WritingPointer >= PacketsCount)
                WritingPointer = 0;
            return true;
        }

        internal bool Read(byte[] aData)
        {
            if (aData.Length != PacketSize)
                return false;

            while (!BufferStatus[ReadingPointer]) ;

            var xAdd = (byte*)(Buffer + ReadingPointer * PacketSize);
            for (int i = 0; i < PacketSize; i++)
                aData[i] = xAdd[i];
            BufferStatus[ReadingPointer] = false;

            ReadingPointer++;
            if (ReadingPointer >= PacketsCount)
                ReadingPointer = 0;
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