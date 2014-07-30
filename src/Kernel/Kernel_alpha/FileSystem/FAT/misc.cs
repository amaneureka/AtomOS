using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kernel_alpha.FileSystem.FAT
{
    /*
     * here all structures/enums etc will coded
    */
    public static class misc
    {
        //Here all helpful fat libs, like ToUInt32...i guess you don't need this class
    }
    public enum FatType : byte
    {
        /// <summary>
        /// Represents a 12-bit FAT.
        /// </summary>
        FAT12 = 12,

        /// <summary>
        /// Represents a 16-bit FAT.
        /// </summary>
        FAT16 = 16,

        /// <summary>
        /// Represents a 32-bit FAT.
        /// </summary>
        FAT32 = 32
    }

    internal struct Entry
    {
        internal const uint DOSName = 0x00; // 8
        internal const uint DOSExtension = 0x08;	// 3
        internal const uint FileAttributes = 0x0B;	// 1
        internal const uint Reserved = 0x0C;	// 1
        internal const uint CreationTimeFine = 0x0D; // 1
        internal const uint CreationTime = 0x0E; // 2
        internal const uint CreationDate = 0x10; // 2
        internal const uint LastAccessDate = 0x12; // 2
        internal const uint EAIndex = 0x14; // 2
        internal const uint LastModifiedTime = 0x16; // 2
        internal const uint LastModifiedDate = 0x18; // 2
        internal const uint FirstCluster = 0x1A; // 2
        internal const uint FileSize = 0x1C; // 4
        internal const uint EntrySize = 32;
    }
}
