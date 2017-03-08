/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Virtual File System
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomixilc.Lib;

using Atomix.Kernel_H.Lib;
using Atomix.Kernel_H.Core;

namespace Atomix.Kernel_H.IO.FileSystem
{
    internal static class VirtualFileSystem
    {
        static IDictionary<string, GenericFileSystem> MountedFS;

        internal static void Setup()
        {
            MountedFS = new IDictionary<string, GenericFileSystem>(Internals.GetHashCode, string.Equals);
        }

        internal static GenericFileSystem GetFS(string aDevice)
        {
            if (!MountedFS.ContainsKey(aDevice))
                return null;
            return MountedFS[aDevice];
        }

        internal static Stream GetFile(string aPath)
        {
            var paths = Marshal.Split(aPath, '/');
            var FileSystem = GetFS(paths[0]);
            if (FileSystem == null)
                return null;
            var xStream = FileSystem.GetFile(paths, 1);
            Heap.FreeArray(paths);
            return xStream;
        }

        internal static bool CreateFile(string aPath)
        {
            var paths = Marshal.Split(aPath, '/');
            var FileSystem = GetFS(paths[0]);
            if (FileSystem == null)
                return false;
            var xValue = FileSystem.CreateFile(paths, 1);
            Heap.FreeArray(paths);
            return xValue;
        }

        internal static bool MountDevice(string aDeviceName, GenericFileSystem aFS)
        {
            if (!aFS.IsValid)
                return false;

            if (aDeviceName == null)
                aDeviceName = GetDeviceLabel();

            if (MountedFS.ContainsKey(aDeviceName))
                return false;

            MountedFS.Add(aDeviceName, aFS);
            Debug.Write("Directory Mounted: %s\n", aDeviceName);
            return true;
        }

        static uint mDeviceLabelCounter = 0;
        private static string GetDeviceLabel()
        {
			string suffix = Convert.ToString(mDeviceLabelCounter++);
            string Label = ("disk") + suffix;
            Heap.Free(suffix);
            return Label;
        }
    }
}
