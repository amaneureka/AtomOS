using System;

using Atomix.Kernel_H.core;

namespace Atomix.Kernel_H.io
{
    public unsafe class Pipe
    {
        private uint Buffer;
        private uint BufferSize;
        private uint PacketSize;
        private uint PacketsCount;
        private bool[] BufferStatus;

        uint ReadingPointer;
        uint WritingPointer;

        public Pipe(uint aPacketSize, uint aPacketsCount)
        {
            this.PacketsCount = aPacketsCount;
            this.PacketSize = aPacketSize;
            this.BufferSize = PacketsCount * PacketSize;
            this.Buffer = Heap.kmalloc(BufferSize);
            this.BufferStatus = new bool[PacketsCount];

            ReadingPointer = WritingPointer = 0;
        }

        public bool Write(byte[] aData, bool Hangup = true)
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

        public bool Read(byte[] aData)
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

        public bool Close()
        {
            Heap.Free(Buffer, BufferSize);
            Heap.Free(BufferStatus);
            return true;
        }
    }
}