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
}
