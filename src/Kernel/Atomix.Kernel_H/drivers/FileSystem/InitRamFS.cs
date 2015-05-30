using System;

using Atomix.Kernel_H.core;

namespace Atomix.Kernel_H.drivers.FileSystem
{
    public unsafe class InitRamFS : GenericFileSystem
    {
        protected UInt32 DiskHash1, DiskHash2, DiskHash3, DiskHash4;

        public InitRamFS(MemoryStream Stream, UInt32 Hash1, UInt32 Hash2, UInt32 Hash3, UInt32 Hash4)
            :base(Stream)
        {
            this.DiskHash1 = Hash1;
            this.DiskHash2 = Hash2;
            this.DiskHash3 = Hash3;
            this.DiskHash4 = Hash4;

            Debug.Write("RamDiskSize::%d\n", Stream.Size);
            if (Stream.Size != 0)
                mValid = true;//IsValid();
        }

        private bool IsValid()
        {
            UInt32 ComHash1 = 0, ComHash2 = 0, ComHash3 = 0xDEAD, ComHash4 = 0;
            for (int i = 0; i < Stream.Size; i++)
            {
                var key = ((MemoryStream)Stream).Address8[i];
                if (key == 0x0)
                {
                    ComHash4++;
                    continue;
                }
                ComHash1 = key + (ComHash1 << 6) + (ComHash1 << 16) - ComHash1;
                ComHash2 = ((MemoryStream)Stream).Address8[Stream.Size - 1 - i] + (ComHash2 << 6) + (ComHash2 << 16) - ComHash2;
                var AND = ComHash3 & key;
                var XOR = ComHash3 ^ key;
                var OR = ComHash3 | key;
                var MAXIMUM = (XOR > OR ? XOR : OR);
                MAXIMUM = MAXIMUM > AND ? MAXIMUM : AND;
                var MINIMUM = (XOR < OR ? XOR : OR);
                MINIMUM = MINIMUM < AND ? MINIMUM : AND;
                MAXIMUM -= MINIMUM;
                ComHash3 += ~(MAXIMUM & (MAXIMUM - 1));
            }
            ComHash4 = Stream.Size - ComHash4;

            Debug.Write("RamFS Computed Hash::%d ", ComHash1);
            Debug.Write("%d ", ComHash2);
            Debug.Write("%d ", ComHash3);
            Debug.Write("%d\n", ComHash4);

            if (ComHash1 != DiskHash1)
                return false;

            if (ComHash2 != DiskHash2)
                return false;

            if (ComHash3 != DiskHash3)
                return false;

            if (ComHash4 != DiskHash4)
                return false;

            return true;
        }

        public override byte[] ReadFile(string FileName)
        {
            return null;
        }

        public override unsafe byte* ReadFile(int EntryNo)
        {
            if (!mValid)
            {
                return null;
            }

            int p = 0, entry_count = -1;
            uint xLocation = 0;
            while (true)
            {
                int len = *((byte*)(((MemoryStream)Stream).Address8 + p));
                if (len == 0)
                    return null;
                p += 1 + len;
                entry_count++;
                if (EntryNo == entry_count)
                {
                    xLocation = *((UInt32*)(((MemoryStream)Stream).Address8 + p)) + (uint)((MemoryStream)Stream).Address8;
                    break;
                }
                p += 8;
            }
            return (byte*)xLocation;
        }

        public override byte* ReadFile(string FileName, out uint xSize)
        {
            if (!mValid)
            {
                xSize = 0;
                return null;
            }

            //It have no directory like structure;
            //It is based on current assumption and can result in some changes in near future
            int p = 0;
            uint xLocation = 0;
            xSize = 0;
            while(xLocation == 0)
            {
                int len = *((byte*)(((MemoryStream)Stream).Address8 + p));
                if (len == 0)
                    break;
                p += 1;
                int i;
                for (i = 0; i < len; i++)
                {
                    if (*((byte*)(((MemoryStream)Stream).Address8 + p + i)) != (byte)FileName[i])
                    {
                        i = -1;
                        break;
                    }
                }
                p += len;

                if (i == -1)
                {
                    p += 8;
                }
                else
                {
                    xLocation = *((UInt32*)(((MemoryStream)Stream).Address8 + p)) + (uint)((MemoryStream)Stream).Address8;
                    xSize = *((UInt32*)(((MemoryStream)Stream).Address8 + p + 4));
                }
            }
            return (byte*)xLocation;
        }
    }
}
