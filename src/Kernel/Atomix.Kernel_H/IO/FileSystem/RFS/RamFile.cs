/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          RFS (Ram File System) Helper
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomix.Kernel_H.IO.FileSystem.RFS
{
    internal class RamFile
    {
        internal readonly string Name;
        internal readonly uint StartAddress;
        internal readonly int Length;

        internal RamFile(string aName, uint aStartAddress, int aLength)
        {
            Name = aName;
            StartAddress = aStartAddress;
            Length = aLength;
        }
    }
}
