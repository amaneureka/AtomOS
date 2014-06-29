using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileSystem.ATA;

namespace FileSystem.FAT
{
    public class Fatfilesystem
    {
        private Partition PartitionX;
        private string VolumeLabel;
        private UInt16 BytePerSector;
        private uint SectorsPerCluster;
        private UInt16 ReservedSector;
        private uint TotalFAT;
        private UInt16 DirectoryEntry;
        private UInt32 TotalSectors;
        private UInt32 SectorsPerFAT;
        private UInt32 DataSectorCount;
        private UInt32 ClusterCount;
        public FatType FatType;
        private UInt32 SerialNo;
        private UInt32 RootCluster;
        private UInt32 RootSector;
        private UInt32 RootSectorCount;
        private UInt32 DataSector;

        public readonly bool IsValid = false;

        public Fatfilesystem(Partition p)
        {
            this.PartitionX = p;
            IsValid = IsFAT();
        }

        private bool IsFAT()
        {
            byte[] BootSector = new byte[512];

            this.PartitionX.Read(0UL, 1U, BootSector);

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
                DataSectorCount = TotalSectors - (ReservedSector + (TotalFAT * SectorsPerFAT) + ReservedSector);
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
            if (FatType == FAT.FatType.FAT32)
            {
                SerialNo = BitConverter.ToUInt32(BootSector, 39);
                byte[] Vlabel = new byte[11];
                for (int i = 71; i < 82; i++)
                {
                    Vlabel[i - 71] = BootSector[i];
                }
                VolumeLabel = Encoding.ASCII.GetString(Vlabel);
                VolumeLabel = VolumeLabel.TrimEnd('\0');
                RootCluster = BitConverter.ToUInt32(BootSector, 44);
                RootSector = 0;
                RootSectorCount = 0;
            }
            /* The key is of another door */
            else
            {
                SerialNo = BitConverter.ToUInt32(BootSector, 67);
                byte[] Vlabel = new byte[11];
                for (int i = 43; i < 54; i++)
                {
                    Vlabel[i - 43] = BootSector[i];
                }
                VolumeLabel = Encoding.ASCII.GetString(Vlabel);
                VolumeLabel = VolumeLabel.TrimEnd('\0');
                RootSector = ReservedSector + (TotalFAT * SectorsPerFAT);
                RootSectorCount = (UInt32)((DirectoryEntry * 32 + (BytePerSector - 1)) / BytePerSector);
            }
            /* Now it shows our forward path ;) */
            DataSector = ReservedSector + (TotalFAT * SectorsPerFAT) + RootSectorCount;
            return true;
        }

        public override string ToString()
        {
            if (IsValid)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("FAT Version:" + FatType);
                sb.AppendLine("Disk Volume:" + (VolumeLabel == "NO NAME" ? VolumeLabel + "<Extended>" : VolumeLabel));
                sb.AppendLine("Bytes Per Sector:" + BytePerSector);
                sb.AppendLine("Sectors Per Cluster:" + SectorsPerCluster);
                sb.AppendLine("Reserved Sector:" + ReservedSector);
                sb.AppendLine("Total FAT:" + TotalFAT);
                sb.AppendLine("Direactory Entry:" + DirectoryEntry);
                sb.AppendLine("Total Sectors:" + TotalSectors);
                sb.AppendLine("Sectors Per FAT:" + SectorsPerFAT);
                sb.AppendLine("Data Sector Count:" + DataSectorCount);
                sb.AppendLine("Cluster Count:" + ClusterCount);
                sb.AppendLine("Serial Number:" + SerialNo);
                sb.AppendLine("Root Cluster:" + RootCluster);
                sb.AppendLine("Root Sector:" + RootSector);
                sb.AppendLine("Root Sector Count:" + RootSectorCount);
                sb.AppendLine("Data Sector:" + DataSector);
                return sb.ToString();
            }
            else
                return "No fat available";
        }
    }    
}
