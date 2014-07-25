using System;
using System.Collections.Generic;
using Kernel_alpha.Drivers.Buses.ATA;
using Kernel_alpha.Drivers;

namespace Kernel_alpha.Drivers.PartInfo
{
    public class MBR
    {
        protected IDE aDisk;
        protected List<Partition> aPartitions;

        public List<Partition> PartInfo
        { get { return aPartitions; } }

        public MBR(IDE mDisk)
        {
            this.aDisk = mDisk;
            this.aPartitions = new List<Partition>();

            var aMBR = new byte[512];
            mDisk.Read(0U, 1U, aMBR);
            ParseData(aMBR, 446);
            ParseData(aMBR, 462);
            ParseData(aMBR, 478);
            ParseData(aMBR, 494);
        }

        private void ParseData(byte[] aMBR, Int32 aLoc)
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
                aPartitions.Add(new Partition(this.aDisk, xStartSector, xSectorCount));
            }
        }
    }
}
