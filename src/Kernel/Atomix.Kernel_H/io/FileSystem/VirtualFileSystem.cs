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
        static IDictionary<GenericFileSystem> MountedFS;

        public static void Setup()
        {
            MountedFS = new IDictionary<GenericFileSystem>();
        }

        public static GenericFileSystem GetFS(string aDevice)
        {
            if (!MountedFS.Contains(aDevice))
                return null;
            return MountedFS[aDevice];
        }

        public static Stream GetFile(string aPath)
        {
            var paths = aPath.Split('/');
            var FileSystem = GetFS(paths[0]);
            if (FileSystem == null)
                return null;
            var xStream = FileSystem.GetFile(paths, 1);
            Heap.FreeArray(paths);
            return xStream;
        }

        public static bool CreateFile(string aPath)
        {
            var paths = aPath.Split('/');
            var FileSystem = GetFS(paths[0]);
            if (FileSystem == null)
                return false;
            var xValue = FileSystem.CreateFile(paths, 1);
            Heap.FreeArray(paths);
            return xValue;
        }

        public static bool MountDevice(string aDeviceName, GenericFileSystem aFS)
        {
            if (!aFS.IsValid)
                return false;

            if (aDeviceName == null)
                aDeviceName = GetDeviceLabel();

            if (MountedFS.Contains(aDeviceName))
                return false;

            MountedFS.Add(aDeviceName, aFS);
            Debug.Write("Directory Mounted: %s\n", aDeviceName);
            return true;
        }

        static uint mDeviceLabelCounter = 0;
        private static string GetDeviceLabel()
        {
            string suffix = (mDeviceLabelCounter++).ToString();
            string Label = ("disk") + suffix;
            Heap.Free(suffix);
            return Label;
        }
    }
}
