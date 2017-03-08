/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Storage Abstract class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomix.Kernel_H.Devices
{
    internal abstract class Storage
    {
        internal abstract unsafe bool Read(uint SectorNo, uint SectorCount, byte* xData);
        internal abstract unsafe bool Write(uint SectorNo, uint SectorCount, byte* xData);

        internal abstract bool Read(uint SectorNo, uint SectorCount, byte[] xData);
        internal abstract bool Write(uint SectorNo, uint SectorCount, byte[] xData);

        internal abstract bool Eject();
    }
}
