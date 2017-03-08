/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Fat File System
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomixilc.Lib;

using Atomix.Kernel_H.Core;
using Atomix.Kernel_H.Devices;
using Atomix.Kernel_H.IO.FileSystem.FAT;
using Atomix.Kernel_H.IO.FileSystem.FAT.Find;

namespace Atomix.Kernel_H.IO.FileSystem
{
    internal class FatFileSystem : GenericFileSystem
    {
        // They should be private set only, so take care of this later
        internal UInt32 BytePerSector;
        internal UInt32 SectorsPerCluster;
        internal UInt32 ReservedSector;
        internal UInt32 TotalFAT;
        internal UInt32 DirectoryEntry;
        internal UInt32 TotalSectors;
        internal UInt32 SectorsPerFAT;
        internal UInt32 DataSectorCount;
        internal UInt32 ClusterCount;
        internal UInt32 SerialNo;
        internal UInt32 RootCluster;
        internal UInt32 RootSector;
        internal UInt32 RootSectorCount;
        internal UInt32 DataSector;
        internal UInt32 EntriesPerSector;
        internal UInt32 fatEntries;

        protected FatType FatType;

        protected string VolumeLabel;

        public FatFileSystem(Storage Device)
        {
            IDevice = Device;
            mIsValid = IsFAT();
        }

        private unsafe bool IsFAT()
        {
            var BootSector = new byte[512];

            if (!IDevice.Read(0U, 1U, BootSector))
            {
                Heap.Free(BootSector);
                return false;
            }

            var xSig = BitConverter.ToUInt16(BootSector, 510);
            if (xSig != 0xAA55)
            {
                Heap.Free(BootSector);
                return false;
            }

            /* BPB (BIOS Parameter Block) */
            BytePerSector = BitConverter.ToUInt16(BootSector, 11);
            SectorsPerCluster = BootSector[13];
            ReservedSector = BitConverter.ToUInt16(BootSector, 14);
            TotalFAT = BootSector[16];
            DirectoryEntry = BitConverter.ToUInt16(BootSector, 17);

            if (BitConverter.ToUInt16(BootSector, 19) == 0)
            {
                /* Large amount of sector on media. This field is set if there are more than 65535 sectors in the volume. */
                TotalSectors = BitConverter.ToUInt32(BootSector, 32);
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
                SectorsPerFAT = BitConverter.ToUInt32(BootSector, 36);
            }

            /* Not Necessary, To Avoid Crashes during corrupted BPB Info */
            // Just to prevent ourself from hacking
            if (TotalFAT == 0 || TotalFAT > 2 || BytePerSector == 0 || TotalSectors == 0 || SectorsPerCluster == 0)
            {
                Heap.Free(BootSector);
                return false;
            }

            /* Some basic calculations to check basic error :P */
            uint RootDirSectors = 0;
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
                SerialNo = BitConverter.ToUInt32(BootSector, 39);
                VolumeLabel = new string((sbyte*)BootSector.GetDataOffset(), 71, 11);   // for checking
                RootCluster = BitConverter.ToUInt32(BootSector, 44);
                RootSector = 0;
                RootSectorCount = 0;
            }
            /* The key is of another door */
            else
            {
                SerialNo = BitConverter.ToUInt32(BootSector, 67);
                VolumeLabel = new string((sbyte*)BootSector.GetDataOffset(), 43, 11);
                RootSector = ReservedSector + (TotalFAT * SectorsPerFAT);
                RootSectorCount = (UInt32)((DirectoryEntry * 32 + (BytePerSector - 1)) / BytePerSector);
                fatEntries = SectorsPerFAT * 512 / 4;
            }
            /* Now it shows our forward path ;) */
            EntriesPerSector = (UInt32)(BytePerSector / 32);
            DataSector = ReservedSector + (TotalFAT * SectorsPerFAT) + RootSectorCount;

            mFSType = FileSystemType.FAT;

            Heap.Free(BootSector);
            return true;
        }

        internal override bool CreateFile(string[] path, int pointer)
        {
            return false;
        }

        internal override Stream GetFile(string[] path, int pointer)
        {
            if (!mIsValid)
                return null;

            FatFileLocation FileLocation = ChangeDirectory(path, pointer);
            if (FileLocation == null)
                return null;

            var xStream = new FatStream(this, path[path.Length - 1], FileLocation.FirstCluster, FileLocation.Size);
            Heap.Free(FileLocation);
            return xStream;
        }

        private FatFileLocation ChangeDirectory(string[] path, int pointer)
        {
            uint CurrentCluster = RootCluster;
            var Compare = new WithName(null);
            FatFileLocation location = null;
            while (pointer < path.Length)
            {
                Compare.Name = path[pointer];
                location = FindEntry(Compare, CurrentCluster);
                if (location == null)
                {
                    Heap.Free(Compare);
                    return null;
                }
                CurrentCluster = location.FirstCluster;
                pointer++;
                Heap.Free(location);
            }

            Heap.Free(Compare);
            return location;
        }

        private FatFileLocation FindEntry(Comparison compare, uint startCluster)
        {
            uint activeSector = ((startCluster - RootCluster) * SectorsPerCluster) + DataSector;

            if (startCluster == 0)
                activeSector = (FatType == FatType.FAT32) ? GetSectorByCluster(RootCluster) : RootSector;

            byte[] aData = new byte[BytePerSector * SectorsPerCluster];
            this.IDevice.Read(activeSector, SectorsPerCluster, aData);

            for (uint index = 0; index < EntriesPerSector * SectorsPerCluster; index++)
            {
                int offset = (int)(index * (int)Entry.EntrySize);
                if (compare.Compare(aData, offset, FatType))
                {
                    FatFileAttributes attribute = (FatFileAttributes)aData[offset + (int)Entry.FileAttributes];
                    Heap.Free(aData);
                    return new FatFileLocation(
                        GetClusterEntry(aData, index, FatType),
                        activeSector,
                        index,
                        (attribute & FatFileAttributes.SubDirectory) != 0,
                        BitConverter.ToInt32(aData, offset + (int)Entry.FileSize));
                }

                if (aData[(int)Entry.DOSName + offset] == (int)FileNameAttribute.LastEntry)
                    break;
            }

            Heap.Free(aData);
            return null;
        }

        internal uint GetClusterEntryValue(uint cluster)
        {
            uint fatoffset = cluster<<2;
            uint sector = ReservedSector + (fatoffset / BytePerSector);
            int sectorOffset = (int)(fatoffset % BytePerSector);

            var aData = new byte[512];
            IDevice.Read(sector, 1U, aData);
            var xNextCluster = (BitConverter.ToUInt32(aData, sectorOffset) & 0x0FFFFFFF);
            Heap.Free(aData);

            return xNextCluster;
        }

        private uint GetSectorByCluster(uint cluster)
        {
            return DataSector + ((cluster - RootCluster) * SectorsPerCluster);
        }

        internal static uint GetClusterEntry(byte[] data, uint index, FatType type)
        {
            uint cluster = BitConverter.ToUInt16(data, (int)((uint)Entry.FirstCluster + (index * (uint)Entry.EntrySize)));

            if (type == FatType.FAT32)
                cluster |= (uint)BitConverter.ToUInt16(data, (int)((uint)Entry.EAIndex + (index * (uint)Entry.EntrySize))) << 16;

            if (cluster == 0)
                cluster = 2;

            return cluster;
        }

        internal static bool IsClusterBad(uint Cluster)
        {
            // Values are depend only on FAT 32 FS
            return (Cluster == 0x0FFFFFF7);
        }

        internal static bool IsClusterFree(uint Cluster)
        {
            // Values are depend only on FAT 32 FS
            return (Cluster == 0x0);
        }

        internal static bool IsClusterReserved(uint Cluster)
        {
            // Values are depend only on FAT 32 FS
            return ((Cluster == 0x0) || (Cluster >= 0xFFF0) && (Cluster < 0x0FFFFFF7));
        }

        internal static bool IsClusterLast(uint Cluster)
        {
            // Values are depend only on FAT 32 FS
            return (Cluster == 0x0FFFFFF8);
        }

        internal byte[] NewBlockArray
        { get { return new byte[SectorsPerCluster * BytePerSector]; } }

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
