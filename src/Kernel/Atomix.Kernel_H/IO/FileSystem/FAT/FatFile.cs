/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          FAT Directory
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

namespace Atomix.Kernel_H.IO.FileSystem.FAT
{
    internal class FatFile : File
    {
        internal readonly FatFileSystem FS;
        internal readonly int StartCluster;
        internal readonly uint Size;

        internal FatFile(FatFileSystem aFS, string aName, int aStartCluster, uint aSize)
            : base(aName)
        {
            FS = aFS;
            StartCluster = aStartCluster;;
            Size = aSize;
        }

        internal override Stream Open(FileMode aMode)
        {
            throw new NotImplementedException();
        }
    }
}
