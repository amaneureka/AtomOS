/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Storage Abstract class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomix.Kernel_H.Devices
{
    internal abstract class Storage
    {
        internal abstract bool Read(uint SectorNo, uint SectorCount, byte[] xData);
        internal abstract bool Write(uint SectorNo, uint SectorCount, byte[] xData);

        internal abstract bool Eject();
    }
}
