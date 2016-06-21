/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Storage Abstract class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

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
