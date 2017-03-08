/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:
* PROGRAMMERS:      SANDEEP ILIGER <sandeep.iliger@gmail.com>
*                   Aman Priyadarshi <aman.eureka@gmail.com>
*/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Kernel_alpha.FileSystem.FAT
{
    public class Details
    {
        public UInt32 Attribute;
        public UInt32 CrtTime;
        public UInt32 CrtDate;
        public UInt32 WrtTime;
        public UInt32 WrtDate;
        public UInt32 FileSize;
        public UInt32 StartCluster;
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

    internal struct ClusterMark
    {
        internal const UInt32 fatMask = 0x0FFFFFFF;
        internal const UInt32 badClusterMark = 0x0FFFFFF7;
        internal const UInt32 endOfClusterMark = 0x0FFFFFF8;
    }

    internal struct FileNameAttribute
    {
        internal const uint LastEntry = 0x00;
        internal const uint Escape = 0x05;	// special msdos hack where 0x05 really means 0xE5 (since 0xE5 was already used for delete)
        internal const uint Dot = 0x2E;
        internal const uint Deleted = 0xE5;
    }

    public enum FatFileAttributes : byte
    {
        /// <summary>
        /// Flag represents the file is read-only.
        /// </summary>
        ReadOnly = 0x01,

        /// <summary>
        /// Flag represents the file is hidden.
        /// </summary>
        Hidden = 0x02,

        /// <summary>
        /// Flag represents the file is a system file.
        /// </summary>
        System = 0x04,

        /// <summary>
        /// Flag represents the file entry is a volume label.
        /// </summary>
        VolumeLabel = 0x08,

        /// <summary>
        /// Flag represents the file entry is a subdirectory.
        /// </summary>
        SubDirectory = 0x10,

        /// <summary>
        /// Flag represents the file has the archive bit set.
        /// </summary>
        Archive = 0x20,

        /// <summary>
        /// Flag represents the file entry is for a device.
        /// </summary>
        Device = 0x40,

        /// <summary>
        /// Flag is unused.
        /// </summary>
        Unused = 0x80,

        /// <summary>
        /// Flag represents the file has a long file name.
        /// </summary>
        LongFileName = 0x0F
    }
}
