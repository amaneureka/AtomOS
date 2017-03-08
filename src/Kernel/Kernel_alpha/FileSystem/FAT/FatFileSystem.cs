/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:
* PROGRAMMERS:      SANDEEP ILIGER <sandeep.iliger@gmail.com>
*                   Aman Priyadarshi <aman.eureka@gmail.com>
*/

using System;
using System.Collections.Generic;
using Kernel_alpha.Drivers;
using Kernel_alpha.FileSystem.FAT;
using Kernel_alpha.Lib.Encoding;
using Kernel_alpha.FileSystem.FAT.Lists;
using Kernel_alpha.Lib;


namespace Kernel_alpha.FileSystem
{
    public class FatFileSystem : GenericFileSystem
    {
        private UInt32 BytePerSector;
        private UInt32 SectorsPerCluster;
        private UInt32 ReservedSector;
        private UInt32 TotalFAT;
        private UInt32 DirectoryEntry;
        private UInt32 TotalSectors;
        private UInt32 SectorsPerFAT;
        private UInt32 DataSectorCount;
        private UInt32 ClusterCount;
        private FatType FatType;
        private UInt32 SerialNo;
        private UInt32 RootCluster;
        private UInt32 RootSector;
        private UInt32 RootSectorCount;
        private UInt32 DataSector;
        private UInt32 EntriesPerSector;
        private UInt32 fatEntries;
        private string VolumeLabel;
        private UInt32 FatCurrentDirectoryEntry;

        public FatFileSystem(BlockDevice aDevice)
        {
            this.IDevice = aDevice;
            this.mIsValid = IsFAT();
        }

        private bool IsFAT()
        {
            var BootSector = new byte[512];
            this.IDevice.Read(0U, 1U, BootSector);

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
                return false;

            /* Some basic calculations to check basic error :P */
            try
            {
                uint RootDirSectors = 0;
                DataSectorCount = TotalSectors - (ReservedSector + (TotalFAT * SectorsPerFAT) + RootDirSectors);
                ClusterCount = DataSectorCount / SectorsPerCluster;
            }
            catch
            {
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

            FatCurrentDirectoryEntry = RootCluster;
            this.mFSType = FileSystemType.FAT;
            return true;
        }

        public override void ChangeDirectory(string DirName)
        {
            if (DirName == null)
                return;

            var location = FindEntry(new FileSystem.Find.WithName(DirName), FatCurrentDirectoryEntry);
            if (location != null)
            {
                FatCurrentDirectoryEntry = location.FirstCluster;
                return;
            }

            throw new Exception("Directory Not Found!");
        }

        public override List<VFS.Entry.Base> ReadDirectory(string DirName = null)
        {
            ChangeDirectory(DirName);

            var xResult = new List<VFS.Entry.Base>();

            byte[] aData = new byte[(UInt32)(512 * SectorsPerCluster)];

            UInt32 xSector = DataSector + ((FatCurrentDirectoryEntry - RootCluster) * SectorsPerCluster);
            this.IDevice.Read(xSector, SectorsPerCluster, aData);

            #region ReadingCode
            uint Entry_offset = 0;
            bool Entry_Type; //True -> Directory & False -> File
            string Entry_Name;
            string Entry_Ext;
            for (uint i = 0; i < SectorsPerCluster * 512; i += 32)
            {
                if (aData[i] == 0x0)
                    break;
                else
                {
                    //Find Entry Type
                    switch (aData[i + 11])
                    {
                        case 0x10:
                            Entry_Type = true;
                            break;
                        case 0x20:
                            Entry_Type = false;
                            break;
                        default:
                            continue;
                    }

                    Entry_offset = i;

                    if (aData[i] != 0xE5)//Entry Exist
                    {
                        Entry_Name = ASCII.GetString(aData, (int)i, 8).Trim();
                        if (!Entry_Type)
                        {
                            Entry_Ext = ASCII.GetString(aData, (int)(i + 8), 3).Trim();
                            xResult.Add(new VFS.Entry.File(Entry_Name + "." + Entry_Ext, BitConverter.ToUInt32(aData, (int)(i + Entry.FileSize))));
                        }
                        else
                        {
                            xResult.Add(new VFS.Entry.Directory(Entry_Name));
                        }
                    }
                }
            }
            #endregion

            return xResult;
        }

        public List<Base> ReadFATDirectory(string DirName = null)
        {
            ChangeDirectory(DirName);

            var xResult = new List<Base>();

            byte[] aData = new byte[(UInt32)(512 * SectorsPerCluster)];

            UInt32 xSector = DataSector + ((FatCurrentDirectoryEntry - RootCluster) * SectorsPerCluster);
            this.IDevice.Read(xSector, SectorsPerCluster, aData);

            #region ReadingCode
            uint Entry_offset = 0;
            bool Entry_Type; //True -> Directory & False -> File
            string Entry_Name;
            string Entry_Ext;
            Details Entry_Detail;
            for (uint i = 0; i < SectorsPerCluster * 512; i += 32)
            {
                if (aData[i] == 0x0)
                    break;
                else
                {
                    //Find Entry Type
                    switch (aData[i + 11])
                    {
                        case 0x10:
                            Entry_Type = true;
                            break;
                        case 0x20:
                            Entry_Type = false;
                            break;
                        default:
                            continue;
                    }

                    Entry_offset = i;

                    if (aData[i] != 0xE5)//Entry Exist
                    {
                        Entry_Detail = new Details();
                        Entry_Name = ASCII.GetString(aData, (int)i, 8).Trim();

                        Entry_Detail.Attribute = 0;
                        Entry_Detail.CrtDate = 0;
                        Entry_Detail.CrtTime = 0;
                        Entry_Detail.FileSize = BitConverter.ToUInt32(aData, (int)(i + Entry.FileSize));
                        Entry_Detail.StartCluster = 0;
                        Entry_Detail.WrtDate = 0;
                        Entry_Detail.WrtTime = 0;

                        if (!Entry_Type)
                        {
                            Entry_Ext = ASCII.GetString(aData, (int)(i + 8), 3).Trim();
                            xResult.Add(new File(Entry_Name + "." + Entry_Ext, Entry_Detail));
                        }
                        else
                        {
                            xResult.Add(new Directory(Entry_Name, Entry_Detail));
                        }
                    }
                }
            }
            #endregion

            return xResult;
        }

        public override void MakeDirectory(string DirName)
        {
            //TODO: Same Entry Exist exception.
            FatFileLocation location = FindEntry(new FileSystem.Find.Empty(), FatCurrentDirectoryEntry);

            uint FirstCluster = AllocateFirstCluster();

            var xdata = new byte[512 * SectorsPerCluster];
            this.IDevice.Read(location.DirectorySector, SectorsPerCluster, xdata);
            BinaryFormat directory = new BinaryFormat(xdata);
            directory.SetString(Entry.DOSName + location.DirectorySectorIndex * 32, "            ", 11);
            directory.SetString(Entry.DOSName + location.DirectorySectorIndex * 32, DirName);

            directory.SetByte(Entry.FileAttributes + location.DirectorySectorIndex * 32, (byte)0x10);
            directory.SetByte(Entry.Reserved + location.DirectorySectorIndex * 32, 0);
            directory.SetByte(Entry.CreationTimeFine + location.DirectorySectorIndex * 32, 0);
            directory.SetUShort(Entry.CreationTime + location.DirectorySectorIndex * 32, 0);
            directory.SetUShort(Entry.CreationDate + location.DirectorySectorIndex * 32, 0);
            directory.SetUShort(Entry.LastAccessDate + location.DirectorySectorIndex * 32, 0);
            directory.SetUShort(Entry.LastModifiedTime + location.DirectorySectorIndex * 32, 0);
            directory.SetUShort(Entry.LastModifiedDate + location.DirectorySectorIndex * 32, 0);
            directory.SetUShort(Entry.FirstCluster + location.DirectorySectorIndex * 32, (ushort)FirstCluster);
            directory.SetUInt(Entry.FileSize + location.DirectorySectorIndex * 32, 0);
            this.IDevice.Write(location.DirectorySector, SectorsPerCluster, xdata);

            FatFileLocation loc = FindEntry(new FileSystem.Find.Empty(), FirstCluster);
            xdata = new byte[512 * SectorsPerCluster];
            this.IDevice.Read(loc.DirectorySector, SectorsPerCluster, xdata);
            directory = new BinaryFormat(xdata);
            for (int i = 0; i < 2; i++)
            {
                directory.SetString(Entry.DOSName + loc.DirectorySectorIndex * 32, "            ", 11);
                if (i == 0)
                {
                    directory.SetString(Entry.DOSName + loc.DirectorySectorIndex * 32, ".");
                    directory.SetUShort(Entry.FirstCluster + loc.DirectorySectorIndex * 32, (ushort)FirstCluster);
                }
                else
                {
                    directory.SetString(Entry.DOSName + loc.DirectorySectorIndex * 32, "..");
                    directory.SetUShort(Entry.FirstCluster + loc.DirectorySectorIndex * 32, (ushort)FatCurrentDirectoryEntry);
                }
                directory.SetByte(Entry.FileAttributes + loc.DirectorySectorIndex * 32, (byte)0x10);
                directory.SetByte(Entry.Reserved + loc.DirectorySectorIndex * 32, 0);
                directory.SetByte(Entry.CreationTimeFine + loc.DirectorySectorIndex * 32, 0);
                directory.SetUShort(Entry.CreationTime + loc.DirectorySectorIndex * 32, 0);
                directory.SetUShort(Entry.CreationDate + loc.DirectorySectorIndex * 32, 0);
                directory.SetUShort(Entry.LastAccessDate + loc.DirectorySectorIndex * 32, 0);
                directory.SetUShort(Entry.LastModifiedTime + loc.DirectorySectorIndex * 32, 0);
                directory.SetUShort(Entry.LastModifiedDate + loc.DirectorySectorIndex * 32, 0);
                directory.SetUInt(Entry.FileSize + loc.DirectorySectorIndex * 32, 0);
                loc.DirectorySectorIndex += 1;
            }

            this.IDevice.Write(loc.DirectorySector, SectorsPerCluster, xdata);
        }

        public override byte[] ReadFile(string FileName)
        {
            byte[] xFileData = new byte[(UInt32)SectorsPerCluster * 512];

            var location = FindEntry(new FileSystem.Find.WithName(FileName), FatCurrentDirectoryEntry);
            if (location == null)
                throw new Exception("File Not Found!");

            byte[] xReturnData = new byte[location.Size];
            UInt32 xSector = DataSector + ((location.FirstCluster - RootCluster) * SectorsPerCluster);
            this.IDevice.Read(xSector, SectorsPerCluster, xFileData);
            Array.Copy(xFileData, 0, xReturnData, 0, location.Size);
            return xReturnData;
        }

        public uint GetSectorByCluster(uint cluster)
        {
            return DataSector + ((cluster - RootCluster) * SectorsPerCluster);
        }

        static public uint GetClusterEntry(byte[] data, uint index, FatType type)
        {
            BinaryFormat entry = new BinaryFormat(data);
            uint cluster = entry.GetUShort(Entry.FirstCluster + (index * Entry.EntrySize));

            if (type == FatType.FAT32)
                cluster |= ((uint)entry.GetUShort(Entry.EAIndex + (index * Entry.EntrySize))) << 16;

            if (cluster == 0)
                cluster = 2;

            return cluster;
        }

        public uint AllocateFirstCluster()
        {
            uint newCluster = AllocateCluster();

            if (newCluster == 0)
                return 0;

            return newCluster;
        }

        protected bool SetClusterEntryValue(uint cluster, uint nextcluster)
        {
            uint fatOffset = 0;

           fatOffset = cluster * 4;

            uint sector = ReservedSector + (fatOffset / BytePerSector);
            uint sectorOffset = fatOffset % BytePerSector;
            uint nbrSectors = 1;

            if ((FatType == FatType.FAT12) && (sectorOffset == BytePerSector - 1))
                nbrSectors = 2;

            var xData = new byte[512 * nbrSectors];
            this.IDevice.Read(sector, nbrSectors, xData);
            BinaryFormat fat = new BinaryFormat(xData);


            fat.SetUInt(sectorOffset, nextcluster);

            this.IDevice.Write(sector, nbrSectors, fat.Data);

            return true;
        }

        protected uint lastFreeHint = 0;
        protected uint AllocateCluster()
        {
            uint at = lastFreeHint + 1;

            if (at < 2)
                at = 2;

            uint last = at - 1;

            if (last == 1)
                last = fatEntries;

            while (at != last)
            {
                uint value = GetClusterEntryValue(at);

                if (IsClusterFree(value))
                {
                    SetClusterEntryValue(at, 0xFFFFFFFF /*endOfClusterMark*/);
                    lastFreeHint = at;
                    return at;
                }

                at++;

                //if (at >= fatEntries)
                //at = 2;
            }

            throw new Exception("No Free Cluster Found!");
            //return 0;	// mean no free space
        }

        protected bool IsClusterFree(uint cluster)
        {
            return ((cluster & ClusterMark.fatMask) == 0x00);
        }

        protected bool IsClusterReserved(uint cluster)
        {
            return (((cluster & ClusterMark.fatMask) == 0x00) || ((cluster & ClusterMark.fatMask) >= ReservedSector) && ((cluster & ClusterMark.fatMask) < ClusterMark.badClusterMark));
        }

        protected bool IsClusterBad(uint cluster)
        {
            return ((cluster & ClusterMark.fatMask) == ClusterMark.badClusterMark);
        }

        protected bool IsClusterLast(uint cluster)
        {
            return ((cluster & ClusterMark.fatMask) >= ClusterMark.endOfClusterMark);
        }

        protected uint GetClusterEntryValue(uint cluster)
        {
            uint fatoffset = 0;


            fatoffset = cluster * 4;

            uint sector = ReservedSector + (fatoffset / BytePerSector);
            uint sectorOffset = fatoffset % BytePerSector;
            uint nbrSectors = 1;

            if ((FatType == FatType.FAT12) && (sectorOffset == BytePerSector - 1))
                nbrSectors = 2;

            var xdata = new byte[512 * nbrSectors];
            this.IDevice.Read(sector, nbrSectors, xdata);
            BinaryFormat fat = new BinaryFormat(xdata);

            uint clusterValue;

             clusterValue = fat.GetUInt(sectorOffset) & 0x0FFFFFFF;

            return clusterValue;
        }


        public FatFileLocation FindEntry(ACompare compare, uint startCluster)
        {
            uint activeSector = ((startCluster - RootCluster) * SectorsPerCluster) + DataSector;

            if (startCluster == 0)
                activeSector = (FatType == FatType.FAT32) ? GetSectorByCluster(RootCluster) : RootSector;

            byte[] aData = new byte[512 * SectorsPerCluster];
            this.IDevice.Read(activeSector, SectorsPerCluster, aData);

            BinaryFormat directory = new BinaryFormat(aData);
            for (uint index = 0; index < EntriesPerSector * SectorsPerCluster; index++)
            {
                Console.WriteLine("Lawl: %d\n" + ((uint)(index * 32)).ToString());
                if (compare.Compare(directory.Data, index * 32, FatType))
                {
                    FatFileAttributes attribute = (FatFileAttributes)directory.GetByte((index * Entry.EntrySize) + Entry.FileAttributes);
                    return new FatFileLocation(GetClusterEntry(directory.Data, index, FatType), activeSector, index, (attribute & FatFileAttributes.SubDirectory) != 0, directory.GetUInt((index * Entry.EntrySize) + Entry.FileSize));
                }

                if (directory.GetByte(Entry.DOSName + (index * Entry.EntrySize)) == FileNameAttribute.LastEntry)
                    return null;
            }
            return null;
        }

        public void FlushDetails()
        {
            if (IsValid)
            {
                Console.WriteLine("FAT Version:" + ((FatType == FatType.FAT32) ? "FAT32" : "FAT16/12"));
                Console.WriteLine("Disk Volume:" + (VolumeLabel == "NO NAME" ? VolumeLabel + "<Extended>" : VolumeLabel));
                Console.WriteLine("Bytes Per Sector:" + BytePerSector.ToString());
                Console.WriteLine("Sectors Per Cluster:" + SectorsPerCluster.ToString());
                Console.WriteLine("Reserved Sector:" + ReservedSector.ToString());
                Console.WriteLine("Total FAT:" + TotalFAT.ToString());
                Console.WriteLine("Direactory Entry:" + DirectoryEntry.ToString());
                Console.WriteLine("Total Sectors:" + TotalSectors.ToString());
                Console.WriteLine("Sectors Per FAT:" + SectorsPerFAT.ToString());
                Console.WriteLine("Data Sector Count:" + DataSectorCount.ToString());
                Console.WriteLine("Cluster Count:" + ClusterCount.ToString());
                Console.WriteLine("Serial Number:" + SerialNo.ToString());
                Console.WriteLine("Root Cluster:" + RootCluster.ToString());
                Console.WriteLine("Root Sector:" + RootSector.ToString());
                Console.WriteLine("Root Sector Count:" + RootSectorCount.ToString());
                Console.WriteLine("Data Sector:" + DataSector.ToString());
            }
            else
                Console.WriteLine("No fat available");
        }
    }
}
