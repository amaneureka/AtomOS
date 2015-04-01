using System;
using System.Collections.Generic;

using Atomix.Kernel_H.core;

namespace Atomix.Kernel_H.drivers.FileSystem
{
    public static class VFS
    {
        public static void Setup()
        {

        }

        public static bool Mount(string root, GenericFileSystem FS)
        {
            if (!FS.IsValidFileSystem)
                return false;
#warning It is working but needs some more code right now
            unsafe
            {
                uint size;
                var xPointer = FS.ReadFile("Hello world.txt", out size);
                Debug.Write("xData of hello world.txt::%d\n", *((UInt32*)xPointer));
            }
            return true;
        }
    }
}
