/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
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
