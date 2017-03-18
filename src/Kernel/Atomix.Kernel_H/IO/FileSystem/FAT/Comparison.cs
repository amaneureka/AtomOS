/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          FAT Comparator
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomix.Kernel_H.IO.FileSystem.FAT
{
    internal abstract class Comparison
    {
        internal abstract bool Compare(byte[] data, int offset, FatType type);
    }
}
