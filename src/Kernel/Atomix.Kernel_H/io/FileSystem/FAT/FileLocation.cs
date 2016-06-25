﻿/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          FAT Helper
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomix.Kernel_H.io.FileSystem.FAT
{
    internal class FatFileLocation
    {
        internal readonly uint FirstCluster;
        internal readonly uint DirectorySector;
        internal readonly uint DirectorySectorIndex;
        internal readonly bool directory;
        internal readonly uint Size;

        internal FatFileLocation(uint aStartCluster, uint aDirectorySector, uint aDirectoryIndex, bool aDirectory, uint aSize)
        {
            FirstCluster = aStartCluster;
            DirectorySector = aDirectorySector;
            DirectorySectorIndex = aDirectoryIndex;
            directory = aDirectory;
            Size = aSize;
        }
    }
}
