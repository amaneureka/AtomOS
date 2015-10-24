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

/*
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

        private int ResourceKey;
        private uint FreeBlocks;

        public Pipe(uint aPacketSize, uint aPacketsCount)
        {
            this.PacketsCount = aPacketsCount;
            this.PacketSize = aPacketSize;
            this.BufferSize = PacketsCount * PacketSize;
            this.Buffer = Heap.kmalloc(BufferSize);
            this.BufferStatus = new bool[PacketsCount];
            this.FreeBlocks = PacketsCount;
            this.ResourceKey = Scheduler.GetResourceID();
        }

        public bool Write(byte[] aData, bool Hangup = true)
        {
            if (aData.Length != PacketSize)
                return false;

            while (Hangup && FreeBlocks == 0) ;

            Scheduler.SpinLock(ResourceKey);
            for (int i = 0; i < PacketsCount; i++)
            {
                if (!BufferStatus[i])
                {
                    FreeBlocks--;
                    BufferStatus[i] = true;
                    var xAdd = (byte*)(Buffer + PacketSize * i);
                    for (uint j = 0; j < PacketSize; j++)
                        xAdd[j] = aData[j];
                    Scheduler.SpinUnlock(ResourceKey);
                    return true;
                }
            }
            var xAddress = (byte*)Buffer;
            for (uint i = PacketSize; i < BufferSize; i++)
                xAddress[i - PacketSize] = xAddress[i];
            xAddress = (byte*)(Buffer + BufferSize - PacketSize);
            for (uint i = 0; i < PacketSize; i++)
                xAddress[i] = aData[i];
            Scheduler.SpinUnlock(ResourceKey);
            return true;
        }

        public bool Read(byte[] aData)
        {
            if (aData.Length != PacketSize)
                return false;

            while (!BufferStatus[0]) ;

            Scheduler.SpinLock(ResourceKey);

            int packet = 0;
            for (packet = 0; packet < PacketsCount; packet++)
                if (!BufferStatus[packet])
                    break;

            FreeBlocks++;
            BufferStatus[packet - 1] = false;
            var xAddress = (byte*)Buffer;
            for (uint i = 0; i < PacketSize; i++)
                aData[i] = xAddress[i];
            for (uint i = PacketSize; i < BufferSize; i++)
                xAddress[i - PacketSize] = xAddress[i];

            Scheduler.SpinUnlock(ResourceKey);
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
*/
