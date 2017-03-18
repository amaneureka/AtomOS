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
        internal readonly uint StartCluster;
        internal readonly uint DirectorySector;
        internal readonly int DirectoryIndex;
        internal readonly int Size;

        internal FatDirectory(FatFileSystem aFS, string aName, uint aStartCluster, uint aDirectorySector, int aDirectoryIndex, int aSize)
            : base(aName)
        {
            FS = aFS;
            StartCluster = aStartCluster;
            DirectorySector = aDirectorySector;
            DirectoryIndex = aDirectoryIndex;
            Size = aSize;
        }

        internal override FSObject FindEntry(string aName)
        {
            return base.FindEntry(aName);
        }
    }
}
