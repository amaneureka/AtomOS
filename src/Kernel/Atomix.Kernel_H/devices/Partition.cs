/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Partition class for storage device
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

namespace Atomix.Kernel_H.Devices
{
    internal class Partition : Storage
    {
        protected Storage mParent;
        protected uint mStartSector;
        protected uint mSectorCount;

        internal Partition(Storage aDisk, uint aStartSector, uint aSectorCount)
        {
            mParent = aDisk;
            mStartSector = aStartSector;
            mSectorCount = aSectorCount;
        }

        internal override bool Read(uint aSectorNo, uint aSectorCount, byte[] aData)
        {
            if (aSectorNo + aSectorCount > mSectorCount)
                return false;
            return mParent.Read(mStartSector + aSectorNo, aSectorCount, aData);
        }

        internal override bool Write(uint aSectorNo, uint aSectorCount, byte[] aData)
        {
            if (aSectorNo + aSectorCount > mSectorCount)
                return false;
            return mParent.Write(mStartSector + aSectorNo, aSectorCount, aData);
        }

        internal override bool Eject()
        {
            return false;
        }
    }
}
