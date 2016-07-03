/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          RFS (Ram File System) Helper
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomix.Kernel_H.IO.FileSystem.RFS
{
    internal class RamFile
    {
        internal readonly string Name;
        internal readonly uint StartAddress;
        internal readonly uint Length;

        internal RamFile(string aName, uint aStartAddress, uint aLength)
        {
            Name = aName;
            StartAddress = aStartAddress;
            Length = aLength;
        }
    }
}
