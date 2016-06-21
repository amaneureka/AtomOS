/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          RFS (Ram File System) Helper
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomix.Kernel_H.io.FileSystem.RFS
{
    public class RamFile
    {
        public readonly string Name;
        public readonly uint StartAddress;
        public readonly uint Length;

        public RamFile(string aName, uint aStartAddress, uint aLength)
        {
            this.Name = aName;
            this.StartAddress = aStartAddress;
            this.Length = aLength;
        }
    }
}
