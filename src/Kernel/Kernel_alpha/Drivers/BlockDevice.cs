using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kernel_alpha.Drivers
{
    public abstract class BlockDevice
    {
        public abstract void Read(UInt32 SectorNo, uint SectorCount, byte[] xData);
        public abstract void Write(UInt32 SectorNo, uint SectorCount, byte[] xData);
    }
}
