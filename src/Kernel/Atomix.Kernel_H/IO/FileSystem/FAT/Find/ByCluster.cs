/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          FAT Helper
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomix.Kernel_H.IO.FileSystem.FAT.Find
{
    internal class ByCluster : Comparison
    {
        internal readonly uint Cluster;
        internal ByCluster(uint aCluster)
        {
            Cluster = aCluster;
        }

        internal override bool Compare(byte[] data, int offset, FatType type)
        {
            switch ((FileNameAttribute)data[offset + (uint)Entry.DOSName])
            {
                case FileNameAttribute.LastEntry:
                case FileNameAttribute.Deleted:
                case FileNameAttribute.Escape:
                case FileNameAttribute.Dot:
                    return false;
                default:
                    break;
            }

            uint startcluster = FatFileSystem.GetClusterEntry(data, (uint)offset, type);
            if (startcluster == Cluster)
                return true;

            return false;
        }
    }
}
