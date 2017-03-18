/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Fat File System
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomixilc.Lib;

using Atomix.Kernel_H.Core;
using Atomix.Kernel_H.IO.FileSystem.FAT;
using Atomix.Kernel_H.IO.FileSystem.FAT.Find;

namespace Atomix.Kernel_H.IO.FileSystem
{
    internal unsafe class FatFileSystem : GenericFileSystem
    {
        int BytePerSector;
        int SectorsPerCluster;
        int ReservedSector;
        int TotalFAT;
        int DirectoryEntry;
        int TotalSectors;
        int SectorsPerFAT;
        int DataSectorCount;
        int ClusterCount;
        int SerialNo;
        int RootCluster;
        int RootSector;
        int RootSectorCount;
        int DataSector;
        int EntriesPerSector;
        int fatEntries;

        FatType FatType;

        string VolumeLabel;

        public FatFileSystem(string aName, Stream aDevice)
            :base(aName, aDevice)
        {
            BytePerSector = 512;
        }

        internal override bool Detect()
        {
            var BootSector = new byte[512];

            if (!ReadBlock(BootSector, 0, 1))
                return false;

            var xSig = BitConverter.ToUInt16(BootSector, 510);
            if (xSig != 0xAA55)
                return false;

            /* BPB (BIOS Parameter Block) */
            BytePerSector = BitConverter.ToUInt16(BootSector, 11);
            SectorsPerCluster = BootSector[13];
            ReservedSector = BitConverter.ToUInt16(BootSector, 14);
            TotalFAT = BootSector[16];
            DirectoryEntry = BitConverter.ToUInt16(BootSector, 17);

            if (BitConverter.ToUInt16(BootSector, 19) == 0)
            {
                /* Large amount of sector on media. This field is set if there are more than 65535 sectors in the volume. */
                TotalSectors = BitConverter.ToInt32(BootSector, 32);
            }
            else
            {
                TotalSectors = BitConverter.ToUInt16(BootSector, 19);
            }

            /* FAT 12 and FAT 16 ONLY */
            SectorsPerFAT = BitConverter.ToUInt16(BootSector, 22);
            if (SectorsPerFAT == 0)
            {
                /* FAT 32 ONLY */
                SectorsPerFAT = BitConverter.ToInt32(BootSector, 36);
            }

            /* Not Necessary, To Avoid Crashes during corrupted BPB Info */
            // Just to prevent ourself from hacking
            if (TotalFAT == 0 || TotalFAT > 2 || BytePerSector == 0 || TotalSectors == 0 || SectorsPerCluster == 0)
                return false;

            /* Some basic calculations to check basic error :P */
            int RootDirSectors = 0;
            DataSectorCount = TotalSectors - (ReservedSector + (TotalFAT * SectorsPerFAT) + RootDirSectors);
            ClusterCount = DataSectorCount / SectorsPerCluster;

            /* Finally we got key xD */
            if (ClusterCount < 4085)
                FatType = FatType.FAT12;
            else if (ClusterCount < 65525)
                FatType = FatType.FAT16;
            else
                FatType = FatType.FAT32;

            /* Now we open door of gold coins xDD */
            if (FatType == FatType.FAT32)
            {
                SerialNo = BitConverter.ToInt32(BootSector, 39);
                VolumeLabel = new string((sbyte*)BootSector.GetDataOffset(), 71, 11);
                RootCluster = BitConverter.ToInt32(BootSector, 44);
                RootSector = 0;
                RootSectorCount = 0;
            }
            /* The key is of another door */
            else
            {
                SerialNo = BitConverter.ToInt32(BootSector, 67);
                VolumeLabel = new string((sbyte*)BootSector.GetDataOffset(), 43, 11);
                RootSector = ReservedSector + (TotalFAT * SectorsPerFAT);
                RootSectorCount = (DirectoryEntry * 32 + (BytePerSector - 1)) / BytePerSector;
                fatEntries = SectorsPerFAT * 512 / 4;
            }

            /* Now it shows our forward path ;) */
            EntriesPerSector = BytePerSector / 32;
            DataSector = ReservedSector + (TotalFAT * SectorsPerFAT) + RootSectorCount;
            return true;
        }

        internal override FSObject FindEntry(string aName)
        {
            throw new NotImplementedException();
        }

        internal FSObject FindEntry(Comparison compare, int startCluster)
        {
            int activeSector = ((startCluster - RootCluster) * SectorsPerCluster) + DataSector;

            if (startCluster == 0)
            {
                activeSector = RootSector;
                if (FatType == FatType.FAT32)
                    activeSector = GetSectorByCluster(RootCluster);
            }

            byte[] aData = new byte[BytePerSector * SectorsPerCluster];
            ReadBlock(aData, activeSector, SectorsPerCluster);

            int offset = 0;
            for (int index = 0; index < EntriesPerSector * SectorsPerCluster; index++)
            {
                offset += (int)Entry.EntrySize;
            }

            return null;
        }

        internal int GetSectorByCluster(int cluster)
        {
            return DataSector + ((cluster - RootCluster) * SectorsPerCluster);
        }

        internal bool ReadBlock(byte[] aBuffer, int aBlockIndex, int aBlockCount)
        {
            int count = BytePerSector * aBlockCount;
            int offset = aBlockIndex * BytePerSector;
            return Device.Read(aBuffer, offset, count) == count;
        }

        internal bool WriteBlock(byte[] aBuffer, int aBlockIndex, int aBlockCount)
        {
            int count = BytePerSector * aBlockCount;
            int offset = aBlockIndex * BytePerSector;
            return Device.Write(aBuffer, offset, count) == count;
        }

        internal void PrintDebugInfo()
        {
            Debug.Write("BytesPerSector\t\t:%d\n", BytePerSector);
            Debug.Write("SectorsPerCluster\t\t:%d\n", SectorsPerCluster);
            Debug.Write("ReservedSector\t\t:%d\n", ReservedSector);
            Debug.Write("TotalFAT\t\t:%d\n", TotalFAT);
            Debug.Write("TotalSectors\t\t:%d\n", TotalSectors);
            Debug.Write("SectorsPerFAT\t\t:%d\n", SectorsPerFAT);
            Debug.Write("DataSectorCount\t\t:%d\n", DataSectorCount);
            Debug.Write("ClusterCount\t\t:%d\n", ClusterCount);
            Debug.Write("SerialNo\t\t:%d\n", SerialNo);
            Debug.Write("RootCluster\t\t:%d\n", RootCluster);
            Debug.Write("RootSector\t\t:%d\n", RootSector);
            Debug.Write("RootSectorCount\t\t:%d\n", RootSectorCount);
            Debug.Write("DataSector\t\t:%d\n", DataSector);
            Debug.Write("EntriesPerSector\t\t:%d\n", EntriesPerSector);
            Debug.Write("fatEntries\t\t:%d\n", fatEntries);
            Debug.Write("VolumeLabel\t\t:%s\n", VolumeLabel);
        }
    }
}
