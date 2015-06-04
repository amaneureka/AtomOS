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
             * │   └───disk
             * └───usr
             */
            ROOT.Add(new Directory("sys"));
            ROOT.Add(new Directory("usr"));
        }
        
        public static Stream Open(string path, FileAttribute fa)
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
                    return null;
                Heap.Free(dir);
            }
            
            dir = paths[c];
            File entry = (File)last.GetEntry(dir);
            
            if (entry == null)
            {
                if (fa == FileAttribute.READ_WRITE
                    || fa == FileAttribute.WRITE_APPEND
                    || fa == FileAttribute.WRITE_ONLY)
                {
                    //Create the file
                    var Alloc = Heap.kmalloc(0x1000);//4KB
                    var stream = new MemoryStream(Alloc, 0x1000);
                    entry = new File(dir, stream);
                    last.Add(entry);
                }
            }
            
            Heap.Free(dir);
            Heap.Free(paths);
            return entry.Open(fa);
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
                    return false;
                Heap.Free(dir);
            }
            dir = paths[c];
            last.Add(new File(dir, stream));
            Heap.Free(dir);
            Heap.Free(paths);

            return true;
        }
    }
}
