/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          FAT Helper
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomix.Kernel_H.IO.FileSystem.FAT.Find
{
    internal class Empty : Comparison
    {
        internal Empty() { }

        internal override bool Compare(byte[] data, int offset, FatType type)
        {
            if ((FileNameAttribute)data[offset + (uint)Entry.DOSName] == FileNameAttribute.LastEntry)
                return true;

            return false;
        }
    }
}
