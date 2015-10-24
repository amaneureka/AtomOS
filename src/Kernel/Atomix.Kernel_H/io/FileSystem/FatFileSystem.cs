using System;

using Atomix.Kernel_H.core;
using Atomix.Kernel_H.devices;
using Atomix.Kernel_H.lib.encoding;
using Atomix.Kernel_H.io.FileSystem.FAT;
using Atomix.Kernel_H.io.FileSystem.FAT.Find;

namespace Atomix.Kernel_H.io.FileSystem
{
    public class FatFileSystem : GenericFileSystem
    {
        protected UInt32 BytePerSector;
        protected UInt32 SectorsPerCluster;
        protected UInt32 ReservedSector;
        protected UInt32 TotalFAT;
        protected UInt32 DirectoryEntry;
        protected UInt32 TotalSectors;
        protected UInt32 SectorsPerFAT;
        protected UInt32 DataSectorCount;
        protected UInt32 ClusterCount;
        protected UInt32 SerialNo;
        protected UInt32 RootCluster;
        protected UInt32 RootSector;
        protected UInt32 RootSectorCount;
        protected UInt32 DataSector;
        protected UInt32 EntriesPerSector;
        protected UInt32 fatEntries;

        protected FatType FatType;

        protected string VolumeLabel;


        public FatFileSystem(Storage Device)
        {
            this.IDevice = Device;
            this.mIsValid = IsFAT();
        }

        private bool IsFAT()
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
            //Just to prevent ourself from hacking
            if (TotalFAT == 0 || TotalFAT > 2 || BytePerSector == 0 || TotalSectors == 0)
            {
                Heap.Free(BootSector);
                return false;
            }

            /* Some basic calculations to check basic error :P */
            try
            {
                uint RootDirSectors = 0;
                DataSectorCount = TotalSectors - (ReservedSector + (TotalFAT * SectorsPerFAT) + RootDirSectors);
                ClusterCount = DataSectorCount / SectorsPerCluster;
            }
            catch
            {
                Heap.Free(BootSector);
                return false;
            }

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
                VolumeLabel = ASCII.GetString(BootSector, 71, 11);   // for checking              
                RootCluster = BitConverter.ToUInt32(BootSector, 44);
                RootSector = 0;
                RootSectorCount = 0;
            }
            /* The key is of another door */
            else
            {
                SerialNo = BitConverter.ToUInt32(BootSector, 67);
                VolumeLabel = ASCII.GetString(BootSector, 43, 11);
                RootSector = ReservedSector + (TotalFAT * SectorsPerFAT);
                RootSectorCount = (UInt32)((DirectoryEntry * 32 + (BytePerSector - 1)) / BytePerSector);
                fatEntries = SectorsPerFAT * 512 / 4;
            }
            /* Now it shows our forward path ;) */
            EntriesPerSector = (UInt32)(BytePerSector / 32);
            DataSector = ReservedSector + (TotalFAT * SectorsPerFAT) + RootSectorCount;

            this.mFSType = FileSystemType.FAT;

            Heap.Free(BootSector);
            return true;
        }

        public override bool CreateFile(string[] path, int pointer)
        {
            return false;
        }

        public override Stream GetFile(string[] path, int pointer)
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
            var Compare = new WithName("");
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
                        BitConverter.ToUInt32(aData, offset + (int)Entry.FileSize));
                }

                if (aData[(int)Entry.DOSName + offset] == (int)FileNameAttribute.LastEntry)
                    break;
            }

            Heap.Free(aData);
            return null;
        }

        private uint GetSectorByCluster(uint cluster)
        {
            return DataSector + ((cluster - RootCluster) * SectorsPerCluster);
        }

        public static uint GetClusterEntry(byte[] data, uint index, FatType type)
        {
            uint cluster = BitConverter.ToUInt16(data, (int)((uint)Entry.FirstCluster + (index * (uint)Entry.EntrySize)));

            if (type == FatType.FAT32)
                cluster |= (uint)BitConverter.ToUInt16(data, (int)((uint)Entry.EAIndex + (index * (uint)Entry.EntrySize))) << 16;

            if (cluster == 0)
                cluster = 2;

            return cluster;
        }

        public byte[] NewBlockArray
        { get { return new byte[SectorsPerCluster * BytePerSector]; } }
    }
}
