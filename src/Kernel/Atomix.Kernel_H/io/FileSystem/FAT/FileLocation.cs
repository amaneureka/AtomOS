/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          FAT Helper
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomix.Kernel_H.io.FileSystem.FAT
{
    public class FatFileLocation
    {
        public readonly uint FirstCluster;
        public readonly uint DirectorySector;
        public readonly uint DirectorySectorIndex;
        public readonly bool directory;
        public readonly uint Size;
        
        public FatFileLocation(uint startCluster, uint directorySector, uint directoryIndex, bool directory, uint size)
        {
            this.FirstCluster = startCluster;
            this.DirectorySector = directorySector;
            this.DirectorySectorIndex = directoryIndex;
            this.directory = directory;
            this.Size = size;
        }
    }
}
