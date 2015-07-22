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

using Atomix.Kernel_H.core;
using Atomix.Kernel_H.io.Streams;
using Atomix.Kernel_H.io.FileSystem.VFS;

namespace Atomix.Kernel_H.io.FileSystem
{
    public static class VirtualFileSystem
    {
        /// <summary>
        /// Root of Virtual File System
        /// </summary>
        private static Directory ROOT;
        
        public static void Setup()
        {
            ROOT = new Directory("\\");
            /*
             * Initial Virtual File system tree
             * //.
             * ├───sys
             * │   ├───dwm
             * │   ├───mouse
             * │   └───disk
             * └───usr
             */
            ROOT.Add(new Directory("sys"));
            ROOT.Add(new Directory("usr"));
        }
        
        public static Stream Open(string path, FileAttribute fa)
        {
            var paths = path.Split('\\');

            Node last = ROOT;
            int c = 0;

            string dir;

            bool SuperNode = false;
            while (c < paths.Length - 1)
            {
                dir = paths[c++];
                last = ((Directory)last).GetEntry(dir);
                if (last is Directory)
                    continue;
                else if (last is SuperNode)
                {                    
                    SuperNode = true;
                    break;
                }
                else
                {
                    Heap.Free(paths);
                    return null;
                }
            }

            if (SuperNode)
            {
                var data = ((SuperNode)last).GetFS.ReadFile(paths, c);
                Heap.Free(paths);
                return new MemoryStream(data, fa);
            }
            else
            {
                dir = paths[c];
                File entry = (File)((Directory)last).GetEntry(dir);
                if (entry == null)
                {
                    if (((int)fa & (int)FileAttribute.CREATE) != 0)
                    {
                        //Create the file
                        var Alloc = Heap.kmalloc(0x1000);//4KB
                        var stream = new MemoryStream(Alloc, 0x1000, fa);
                        entry = new File(dir, stream);
                        ((Directory)last).Add(entry);
                    }
                    else
                    {
                        Heap.Free(paths);
                        return null;
                    }
                }
                Heap.Free(paths);
                return entry.Open(fa);
            }
        }
        
        public static bool Mount(string path, Stream stream)
        {
            var paths = path.Split('\\');

            Directory last = ROOT;
            int c = 0;

            string dir;
            while (c < paths.Length - 1)
            {
                dir = paths[c++];
                Node curr = last.GetEntry(dir);
                if (curr is Directory)
                    last = (Directory)curr;
                else
                {
                    Heap.Free(paths);
                    return false;
                }
            }
            dir = paths[c];
            last.Add(new File(dir, stream));
            Heap.Free(paths);
            return true;
        }

        public static bool Mount(string path, GenericFileSystem FS)
        {
            var paths = path.Split('\\');

            Directory last = ROOT;
            int c = 0;

            string dir;
            while (c < paths.Length - 1)
            {
                dir = paths[c++];
                Node curr = last.GetEntry(dir);
                if (curr is Directory)
                    last = (Directory)curr;
                else
                {
                    Heap.Free(paths);
                    return false;
                }
            }
            dir = paths[c];
            last.Add(new SuperNode(dir, FS));
            Heap.Free(paths);
            return true;
        }
    }
}
