/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is strictly prohibited
*                   Proprietary and confidential
* PURPOSE:          To parse Master Boot Record of storage device
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomix.Kernel_H.lib;
using Atomix.Kernel_H.core;

namespace Atomix.Kernel_H.devices
{
    public class MBR
    {
        protected Storage aDisk;
        protected IList<Partition> aPartitions;

        public IList<Partition> PartInfo
        { get { return aPartitions; } }

        public MBR(Storage mDisk)
        {
            this.aDisk = mDisk;
            this.aPartitions = new IList<Partition>();

            var aMBR = new byte[512];
            mDisk.Read(0U, 1U, aMBR);
            ParseData(aMBR, 446);
            ParseData(aMBR, 462);
            ParseData(aMBR, 478);
            ParseData(aMBR, 494);
            Heap.Free(aMBR);
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
                Debug.Write("[MBR]: EBR Partition Found!\n");
            }
            else if (xSystemID != 0)
            {
                UInt32 xSectorCount = BitConverter.ToUInt32(aMBR, aLoc + 12);
                UInt32 xStartSector = BitConverter.ToUInt32(aMBR, aLoc + 8);
                aPartitions.Add(new Partition(this.aDisk, xStartSector, xSectorCount));
            }
        }

        public void Clean()
        {
            Heap.Free(aPartitions);
            Heap.Free(this);
        }
    }
}
