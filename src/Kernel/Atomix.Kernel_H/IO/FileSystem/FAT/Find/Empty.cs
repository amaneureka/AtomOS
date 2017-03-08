/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
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
