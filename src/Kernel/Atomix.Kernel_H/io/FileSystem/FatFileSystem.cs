using System;

using Atomix.Kernel_H.core;
using Atomix.Kernel_H.devices;
using Atomix.Kernel_H.lib.encoding;
using Atomix.Kernel_H.io.FileSystem.FAT;

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

        public override bool ReadFile(string[] paths, int c, byte[] xReturnData, int index)
        {
            string dir;
            UInt32 CurrentDirectory = RootCluster;
            while (c < paths.Length - 1)
            {
                dir = paths[c++];
                var loc = FindEntry(CurrentDirectory, FAT.FindEntry.WithName, dir);
                if (loc == null)
                    return false;
                CurrentDirectory = loc.FirstCluster;
                Heap.Free(loc);
            }
            dir = paths[c];
            var file = FindEntry(CurrentDirectory, FAT.FindEntry.WithName, dir);
            
            byte[] xFileData = new byte[(UInt32)SectorsPerCluster * 512];
            UInt32 xSector = DataSector + ((file.FirstCluster - RootCluster) * SectorsPerCluster);
            this.IDevice.Read(xSector, SectorsPerCluster, xFileData);

            int filesize = Math.Min((int)(file.Size - index), xReturnData.Length);
            Array.Copy(xFileData, 0, xReturnData, 0, filesize);
            Heap.Free(xFileData);
            Heap.Free(file);
            return true;
        }

        public override byte[] ReadFile(string[] paths, int c)
        {
            string dir;
            UInt32 CurrentDirectory = RootCluster;
            while (c < paths.Length - 1)
            {
                dir = paths[c++];
                var loc = FindEntry(CurrentDirectory, FAT.FindEntry.WithName, dir);
                if (loc == null)
                    return null;
                CurrentDirectory = loc.FirstCluster;
                Heap.Free(loc);
            }
            dir = paths[c];
            var file = FindEntry(CurrentDirectory, FAT.FindEntry.WithName, dir);

            byte[] xFileData = new byte[(UInt32)SectorsPerCluster * 512];
            UInt32 xSector = DataSector + ((file.FirstCluster - RootCluster) * SectorsPerCluster);
            this.IDevice.Read(xSector, SectorsPerCluster, xFileData);

            byte[] xReturnData = new byte[file.Size];
            
            Array.Copy(xFileData, 0, xReturnData, 0, (int)file.Size);
            Heap.Free(xFileData);
            Heap.Free(file);
            return xReturnData;
        }

        public FatFileLocation FindEntry(uint startCluster, FindEntry entryType, object arg0)
        {
            uint activeSector = ((startCluster - RootCluster) * SectorsPerCluster) + DataSector;

            if (startCluster == 0)
                activeSector = (FatType == FatType.FAT32) ? GetSectorByCluster(RootCluster) : RootSector;

            byte[] aData = new byte[512 * SectorsPerCluster];
            this.IDevice.Read(activeSector, SectorsPerCluster, aData);

            FatFileLocation ResultEntry = null;
            for (uint index = 0; index < EntriesPerSector * SectorsPerCluster; index++)
            {
                bool result;
                switch(entryType)
                {
                    case FAT.FindEntry.Any:
                        result = Find.Any(aData, index * 32, FatType);
                        break;
                    case FAT.FindEntry.ByCluster:
                        result = Find.ByCluster(aData, index * 32, FatType, (uint)arg0);
                        break;
                    case FAT.FindEntry.Empty:
                        result = Find.Empty(aData, index * 32, FatType);
                        break;
                    case FAT.FindEntry.WithName:
                        result = Find.WithName(aData, index * 32, FatType, (string)arg0);
                        break;
                    default:
                        result = false;
                        break;
                }
                if (result)
                {
                    var attrib = (FatFileAttributes)aData[((index * (uint)Entry.EntrySize) + (uint)Entry.FileAttributes)];
                    ResultEntry = new FatFileLocation(
                        GetClusterEntry(aData, index, FatType), 
                        activeSector,
                        index,
                        (attrib & FatFileAttributes.SubDirectory) != 0, 
                        BitConverter.ToUInt32(aData, (int)((index * (uint)Entry.EntrySize) + (uint)Entry.FileSize)));
                    break;
                }
            }
            Heap.Free(aData);
            return ResultEntry;
        }

        private uint GetSectorByCluster(uint cluster)
        {
            return DataSector + ((cluster - RootCluster) * SectorsPerCluster);
        }

        public static uint GetClusterEntry(byte[] data, uint index, FatType type)
        {
            uint cluster = BitConverter.ToUInt16(data, (int)((uint)Entry.FirstCluster + (index * (uint)Entry.EntrySize)));

            if (type == FatType.FAT32)
                cluster |=  (uint)BitConverter.ToUInt16(data, (int)((uint)Entry.EAIndex + (index * (uint)Entry.EntrySize))) << 16;

            if (cluster == 0)
                cluster = 2;

            return cluster;
        }
    }
}
