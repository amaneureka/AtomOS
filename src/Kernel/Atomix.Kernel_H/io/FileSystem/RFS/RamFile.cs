using System;

using Atomix.Kernel_H.core;

using Atomix.Kernel_H.lib;

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
