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
        internal readonly uint StartCluster;
        internal readonly uint DirectorySector;
        internal readonly int DirectoryIndex;
        internal readonly int Size;

        internal FatFile(string aName, uint aStartCluster, uint aDirectorySector, int aDirectoryIndex, int aSize)
            : base(aName)
        {
            StartCluster = aStartCluster;
            DirectorySector = aDirectorySector;
            DirectoryIndex = aDirectoryIndex;
            Size = aSize;
        }

        internal override Stream Open(FileMode aMode)
        {
            throw new NotImplementedException();
        }
    }
}
