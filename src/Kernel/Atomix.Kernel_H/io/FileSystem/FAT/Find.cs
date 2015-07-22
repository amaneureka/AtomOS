using System;

using Atomix.Kernel_H.core;
using Atomix.Kernel_H.lib.encoding;

namespace Atomix.Kernel_H.io.FileSystem.FAT
{
    public static class Find
    {
        public static bool Any(byte[] data, uint offset, FatType type)
        {
            switch((FileNameAttribute)data[offset + (uint)Entry.DOSName])
            {
                case FileNameAttribute.LastEntry:
                case FileNameAttribute.Deleted:
                case FileNameAttribute.Escape:
                case FileNameAttribute.Dot:
                    return false;
                default:
                    return true;
            }
        }

        public static bool ByCluster(byte[] data, uint offset, FatType type, uint cluster)
        {
            if (!Any(data, offset, type))
                return false;

            uint startcluster = FatFileSystem.GetClusterEntry(data, offset, type);
            if (startcluster == cluster)
                return true;

            return false;
        }

        public static bool Empty(byte[] data, uint offset, FatType type)
        {
            if ((FileNameAttribute)data[offset + (uint)Entry.DOSName] == FileNameAttribute.LastEntry)
                return true;

            return false;
        }

        public static bool WithName(byte[] data, uint offset, FatType type, string name)
        {
#warning Hey sandeep, why we are not checking for dot here?
            switch ((FileNameAttribute)data[offset + (uint)Entry.DOSName])
            {
                case FileNameAttribute.LastEntry:
                case FileNameAttribute.Deleted:
                case FileNameAttribute.Escape:
                //case FileNameAttribute.Dot: --> this one?
                    return false;
                default:
                    break;
            }

            var FullName = ConcatFullName(data, offset, 11);
            if (FullName == name)
                return true;
            return false;
        }

        private static string ConcatFullName(byte[] data, uint index, int length)
        {
            int newlen = 0;
            for (int i = 0; i < length; i++)
                if (data[index + i] != 0x0)
                    newlen++;

            var xResult = new char[newlen];

            int p = 0;
            for (int i = 0; i < length; i++)
            {
                var bb = data[index + i];
                if (bb != 0x0)
                    xResult[p++] = (char)bb;
            }

            var name = new String(xResult);
            Heap.Free(xResult);
            return name;
        }
    }
}
