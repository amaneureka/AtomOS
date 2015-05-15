using System;
using System.Collections.Generic;

using Atomix.Kernel_H.core;
using Atomix.Kernel_H.drivers.FileSystem.VFS;

namespace Atomix.Kernel_H.drivers.FileSystem
{
    public static class VirtualFileSystem
    {
        private static Node ROOT;
        public static void Setup()
        {
            ROOT = new Node("/");
        }


        private static GenericFileSystem FSs;
        public static GenericFileSystem GetFS()
        {
            return FSs;
        }

        public static bool Mount(string root, GenericFileSystem FS)
        {
            if (!FS.IsValidFileSystem)
            {
                Debug.Write("Invalid FileSystem: ");
                Debug.Write(root);
                Debug.Write('\n');
                return false;
            }
#warning It is working but needs some more code right now
            FSs = FS;
            /*unsafe
            {
                uint size;
                var xPointer = FS.ReadFile("Hello world.txt", out size);
                Debug.Write("xData of hello world.txt::%d\n", *((UInt32*)xPointer));
            }*/
            return true;
        }
    }
}
