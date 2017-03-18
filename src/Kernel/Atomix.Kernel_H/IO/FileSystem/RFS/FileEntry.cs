/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Ram File Entry
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System.Runtime.InteropServices;

namespace Atomix.Kernel_H.IO.FileSystem.RFS
{
    [StructLayout(LayoutKind.Explicit, Size = 40)]
    internal unsafe struct FileEntry
    {
        [FieldOffset(0)]
        public fixed sbyte Name[32];

        [FieldOffset(32)]
        public uint StartAddress;

        [FieldOffset(36)]
        public int Length;
    }
}
