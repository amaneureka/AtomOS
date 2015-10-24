/* Copyright (C) Atomix Development, Inc - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Aman Priyadarshi <aman.eureka@gmail.com>, December 2014
 * 
 * VirtualFileSystem.cs
 *      An abstract layer between different file systems and streams
 *      
 *      History:
 *          16-05-15    VFS Support    Aman Priyadarshi
 */

using System;

using Atomix.Kernel_H.lib;
using Atomix.Kernel_H.core;

namespace Atomix.Kernel_H.io.FileSystem
{
    public static class VirtualFileSystem
    {
        static IList<Pair<string, GenericFileSystem>> MountedFS;

        public static void Setup()
        {
            MountedFS = new IList<Pair<string, GenericFileSystem>>();
            MountedFS.Add(new Pair<string, GenericFileSystem>("ram", new RamFileSystem()));
        }

        public static GenericFileSystem GetFS(string aDevice)
        {
            for (int i = 0; i < MountedFS.Count; i++)
                if (MountedFS[i].First == aDevice)
                    return MountedFS[i].Second;
            return null;
        }

        public static Stream GetFile(string aPath)
        {
            var paths = aPath.Split('\\');
            var FileSystem = GetFS(paths[0]);
            if (FileSystem == null)
                return null;
            var xStream = FileSystem.GetFile(paths, 1);
            Heap.Free(paths);
            return xStream;
        }

        public static bool CreateFile(string aPath)
        {
            var paths = aPath.Split('\\');
            var FileSystem = GetFS(paths[0]);
            if (FileSystem == null)
                return false;
            var xValue = FileSystem.CreateFile(paths, 1);
            Heap.Free(paths);
            return xValue;
        }

        public static bool Mount(string aDeviceName, GenericFileSystem aFS)
        {
            if (!aFS.IsValid)
                return false;
            for (int i = 0; i < MountedFS.Count; i++)
                if (MountedFS[i].First == aDeviceName)
                    return false;
            MountedFS.Add(new Pair<string, GenericFileSystem>(aDeviceName, aFS));
            return true;
        }
    }
}
