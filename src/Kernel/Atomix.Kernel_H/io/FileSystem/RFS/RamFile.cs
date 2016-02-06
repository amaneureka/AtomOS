using System;

using Atomix.Kernel_H.core;

using Atomix.Kernel_H.lib;

namespace Atomix.Kernel_H.io.FileSystem.RFS
{
    public class RamFile
    {
        public readonly string Name;
        public readonly uint StartAddress;
        public readonly int Length;

        public RamFile(string aName, uint aStartAddress, int aLength)
        {
            this.Name = aName;
            this.StartAddress = aStartAddress;
            this.Length = aLength;
        }
    }
}
