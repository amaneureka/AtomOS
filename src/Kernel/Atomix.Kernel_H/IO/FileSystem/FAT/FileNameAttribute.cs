/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          FAT Name Attribute Enum
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomix.Kernel_H.IO.FileSystem.FAT
{
    internal enum FileNameAttribute : uint
    {
        LastEntry = 0x00,
        /* special msdos hack where 0x05 really means 0xE5 (since 0xE5 was already used for delete) */
        Escape = 0x05,
        Dot = 0x2E,
        Deleted = 0xE5,
    };
}
