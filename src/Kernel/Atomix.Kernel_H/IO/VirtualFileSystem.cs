/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Virtual File System
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomix.Kernel_H.Lib;
using Atomix.Kernel_H.Core;

namespace Atomix.Kernel_H.IO
{
    internal static class VirtualFileSystem
    {
        static VFN mRoot;

        internal static void Install()
        {
            mRoot = new VFN(string.Empty);
        }

        internal static FSObject Open(string aPath)
        {
            var paths = Marshal.Split(aPath, '/');

            int count = paths.Length;
            if (paths[0] != string.Empty)
                return null;

            FSObject node = mRoot;
            for (int i = 1; i < count; i++)
            {
                if (node == null || !(node is Directory))
                    return null;
                node = ((Directory)node).FindEntry(paths[i]);
            }

            return node;
        }

        internal static bool Mount(FSObject aObject, string aPath)
        {
            var node = Open(aPath);
            if (node == null || !(node is VFN))
                return false;
            Debug.Write("VFS Mounted: %s\n", aPath);
            ((VFN)node).Mount(aObject);
            return true;
        }

        internal static bool Map(string aPath, string aName)
        {
            return Mount(new VFN(aName), aPath);
        }
    }
}