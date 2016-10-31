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
    public abstract class Storage
    {
        public abstract bool Read(uint SectorNo, uint SectorCount, byte[] xData);
        public abstract bool Write(uint SectorNo, uint SectorCount, byte[] xData);

        public abstract bool Eject();
    }
}
