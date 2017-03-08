/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          FAT Helper
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomix.Kernel_H.IO.FileSystem.FAT
{
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

    internal enum Entry : int
    {
        DOSName = 0x00, // 8
        DOSExtension = 0x08,	// 3
        FileAttributes = 0x0B,	// 1
        Reserved = 0x0C,	// 1
        CreationTimeFine = 0x0D, // 1
        CreationTime = 0x0E, // 2
        CreationDate = 0x10, // 2
        LastAccessDate = 0x12, // 2
        EAIndex = 0x14, // 2
        LastModifiedTime = 0x16, // 2
        LastModifiedDate = 0x18, // 2
        FirstCluster = 0x1A, // 2
        FileSize = 0x1C, // 4
        EntrySize = 32,
    }

    internal enum FindEntry
    {
        Any = 0x1,
        ByCluster = 0x2,
        Empty = 0x3,
        WithName = 0x4,
    }

    internal enum FileNameAttribute : uint
    {
        LastEntry = 0x00,
        Escape = 0x05,	// special msdos hack where 0x05 really means 0xE5 (since 0xE5 was already used for delete)
        Dot = 0x2E,
        Deleted = 0xE5,
    }

    internal enum FatFileAttributes : byte
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

    internal abstract class Comparison
    {
        internal abstract bool Compare(byte[] data, int offset, FatType type);
    }
}
