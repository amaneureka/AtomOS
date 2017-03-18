/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          FAT Directory
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomix.Kernel_H.IO.FileSystem.FAT
{
    internal class FatDirectory : Directory
    {
        internal readonly FatFileSystem FS;
        internal readonly int StartCluster;
        internal readonly uint Size;

        internal FatDirectory(FatFileSystem aFS, string aName, int aStartCluster, uint aSize)
            : base(aName)
        {
            FS = aFS;
            StartCluster = aStartCluster;
            Size = aSize;
        }

        internal override FSObject FindEntry(string aName)
        {
            return FS.FindEntry(aName, StartCluster);
        }
    }
}
