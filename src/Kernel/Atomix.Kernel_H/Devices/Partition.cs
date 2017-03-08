/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Partition class for storage device
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

namespace Atomix.Kernel_H.Devices
{
    internal unsafe class Partition : Storage
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

        internal override bool Read(uint aSectorNo, uint aSectorCount, byte* aData)
        {
            return mParent.Read(mStartSector + aSectorNo, aSectorCount, aData);
        }

        internal override bool Write(uint aSectorNo, uint aSectorCount, byte[] aData)
        {
            if (aSectorNo + aSectorCount > mSectorCount)
                return false;
            return mParent.Write(mStartSector + aSectorNo, aSectorCount, aData);
        }

        internal override bool Write(uint aSectorNo, uint aSectorCount, byte* aData)
        {
            return mParent.Write(mStartSector + aSectorNo, aSectorCount, aData);
        }

        internal override bool Eject()
        {
            return false;
        }
    }
}
