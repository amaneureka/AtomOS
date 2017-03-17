/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          FAT File Attribute Enum
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/


namespace Atomix.Kernel_H.IO.FileSystem.FAT
{
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
    };
}
