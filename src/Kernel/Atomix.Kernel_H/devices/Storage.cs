using System;

namespace Atomix.Kernel_H.devices
{
    public abstract class Storage
    {
        public abstract bool Read(UInt32 SectorNo, uint SectorCount, byte[] xData);
        public abstract bool Write(UInt32 SectorNo, uint SectorCount, byte[] xData);

        public abstract bool Eject();
    }
}
