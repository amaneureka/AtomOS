/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          FAT Entry Structure
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System.Runtime.InteropServices;

namespace Atomix.Kernel_H.IO.FileSystem.FAT
{
    [StructLayout(LayoutKind.Explicit, Size = 40)]
    internal unsafe struct Entry
    {
        [FieldOffset(0)]
        public fixed sbyte DOSName[8];

        [FieldOffset(8)]
        public fixed sbyte DOSExtension[3];

        [FieldOffset(11)]
        public FatFileAttributes FileAttributes;

        [FieldOffset(12)]
        public byte Reserved;

        [FieldOffset(13)]
        public byte CreationTimeFine;

        [FieldOffset(14)]
        public short CreationTime;

        [FieldOffset(16)]
        public short CreationDate;

        [FieldOffset(18)]
        public short LastAccessDate;

        [FieldOffset(20)]
        public short EAIndex;

        [FieldOffset(22)]
        public short LastModifiedTime;

        [FieldOffset(24)]
        public short LastModifiedDate;

        [FieldOffset(26)]
        public short FirstCluster;

        [FieldOffset(28)]
        public uint FileSize;

        [FieldOffset(32)]
        public uint EntrySize;
    };
}
