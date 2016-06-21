/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Partition class for storage device
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

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
            if (SectorNo + SectorCount > aSectorCount)
                return false;
            return aParent.Read(aStartSector + SectorNo, SectorCount, xData);
        }

        public override bool Write(uint SectorNo, uint SectorCount, byte[] xData)
        {
            if (SectorNo + SectorCount > aSectorCount)
                return false;
            return aParent.Write(aStartSector + SectorNo, SectorCount, xData);
        }

        public override bool Eject()
        {
            return false;
        }
    }
}
