/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          FAT Type Enum
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
}
