/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          FAT Entry Enum
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/


namespace Atomix.Kernel_H.IO.FileSystem.FAT
{
    internal enum Entry : int
    {
        DOSName             = 0x00, // 8
        DOSExtension        = 0x08,	// 3
        FileAttributes      = 0x0B,	// 1
        Reserved            = 0x0C,	// 1
        CreationTimeFine    = 0x0D, // 1
        CreationTime        = 0x0E, // 2
        CreationDate        = 0x10, // 2
        LastAccessDate      = 0x12, // 2
        EAIndex             = 0x14, // 2
        LastModifiedTime    = 0x16, // 2
        LastModifiedDate    = 0x18, // 2
        FirstCluster        = 0x1A, // 2
        FileSize            = 0x1C, // 4
        EntrySize           = 32,
    }
}
