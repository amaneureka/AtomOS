using System;
using System.Collections.Generic;

namespace Atomix.Kernel_H.io.FileSystem.FAT.Find
{
    public class ByCluster : Comparison
    {
        public readonly uint Cluster;
        public ByCluster(uint aCluster)
        {
            this.Cluster = aCluster;
        }

        public override bool Compare(byte[] data, int offset, FatType type)
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
