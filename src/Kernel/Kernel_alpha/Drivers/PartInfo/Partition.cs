using System;
using System.Collections.Generic;
using Kernel_alpha.Drivers.Buses.ATA;

namespace Kernel_alpha.Drivers
{
    public class Partition : BlockDevice
    {
        protected IDE aDisk;
        protected UInt32 aStartSector;
        protected UInt32 aSectorCount;

        public Partition(IDE Disk, UInt32 StartSector, UInt32 SectorCount)
        {
            this.aDisk = Disk;
            this.aStartSector = StartSector;
            this.aSectorCount = SectorCount;
        }

        public override void Read(UInt32 BlockNo, UInt32 BlockCount, byte[] aData)
        {
            #warning Add Overflow exception
            aDisk.Read(BlockNo + aStartSector, BlockCount, aData);
        }

        public override void Write(UInt32 BlockNo, UInt32 BlockCount, byte[] aData)
        {
            #warning Add Overflow exception
            aDisk.Write(BlockNo + aStartSector, BlockCount, aData);
        }
    }
}
