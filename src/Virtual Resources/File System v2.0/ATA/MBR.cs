using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FileSystem;

namespace FileSystem.ATA
{
    public class MBR
    {
        protected BlockDevice IDisk;

        public MBR(BlockDevice disk)
        {
            this.IDisk = disk;

            byte[] aMBR = new byte[512];
            IDisk.Read(0UL, 1U, aMBR);

            //Do parse for all four entries
            ParsePartition(aMBR, 446);
            ParsePartition(aMBR, 462);
            ParsePartition(aMBR, 478);
            ParsePartition(aMBR, 494);
        }

        private void ParsePartition(byte[] aMBR, int aLoc)
        {
            byte xSystemID = aMBR[aLoc + 4];
            if (xSystemID == 0x5 || xSystemID == 0xF || xSystemID == 0x85)
            {
                //Extended Partition Detected
                //DOS only knows about 05, Windows 95 introduced 0F, Linux introduced 85 
                //Search for logical volumes
                //http://thestarman.pcministry.com/asm/mbr/PartTables2.htm          
            }
            else if (xSystemID != 0)
            {
                UInt32 xSectorCount = BitConverter.ToUInt32(aMBR, aLoc + 12);
                UInt32 xStartSector = BitConverter.ToUInt32(aMBR, aLoc + 8);

                Stream Part = IDisk.IDevice;
                Part.Seek((xStartSector * 512) + 0x20000, SeekOrigin.Begin);
                var xPart = new Partition((Atapio)IDisk, Part, xSectorCount); //For now assume it as ATAPIO device only                
            }
        }
    }
}
