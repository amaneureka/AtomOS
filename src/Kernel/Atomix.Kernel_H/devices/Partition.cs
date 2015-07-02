using System;

namespace Atomix.Kernel_H.devices
{
    public class Partition : Storage
    {
        protected Storage aParent;
        protected UInt32 aStartSector;
        protected UInt32 aSectorCount;

        public Partition(Storage Disk, UInt32 StartSector, UInt32 SectorCount)
        {
            this.aParent = Disk;
            this.aStartSector = StartSector;
            this.aSectorCount = SectorCount;
        }

        public override bool Read(uint SectorNo, uint SectorCount, byte[] xData)
        {
            if (SectorNo > SectorCount)
                return false;
            return aParent.Read(aStartSector + SectorNo, SectorCount, xData);
        }

        public override bool Write(uint SectorNo, uint SectorCount, byte[] xData)
        {
            if (SectorNo > SectorCount)
                return false;
            return aParent.Write(aStartSector + SectorNo, SectorCount, xData);
        }
    }
}
