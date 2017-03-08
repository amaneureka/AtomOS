/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
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
            return Write((byte*)aData.GetDataOffset(), Hangup);
        }

        internal bool Write(byte* aData, bool Hangup = true)
        {
            while (Hangup && BufferStatus[WritingPointer])
                Task.Switch();

            if (BufferStatus[WritingPointer])
                return false;

            Memory.FastCopy(Buffer + (uint)(WritingPointer * PacketSize), (uint)aData, (uint)PacketSize);
            BufferStatus[WritingPointer] = true;

            WritingPointer = (WritingPointer + 1) % PacketsCount;
            return true;
        }

        internal bool Read(byte[] aData)
        {
            while (!BufferStatus[ReadingPointer])
                Task.Switch();

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