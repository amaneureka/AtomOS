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
using Atomix.Kernel_H.drivers.FileSystem.VFS;

namespace Atomix.Kernel_H.drivers.FileSystem
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
             * │   ├───RamFS
             * │   └───*
             * └───*
             */
            ROOT.Add(new Directory("sys"));
        }
        
        public static GenericFileSystem GetFS(string root)
        {
            var paths = root.Split('\\');
            Directory Curr = ROOT;
            int c = 0;
            while (c < paths.Length - 1)
            {
#warning No Checking of real base class
                Curr = (Directory)Curr.GetEntry(paths[c++]);
            }
            return (GenericFileSystem)(((SuperNode)((Directory)Curr).GetEntry(paths[c])).Open());
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
            
            var paths = root.Split('\\');
            Directory Curr = ROOT;
            int c = 0;
            while(c < paths.Length - 1)
            {
#warning No Checking of real base class
                Curr = (Directory)Curr.GetEntry(paths[c++]);
            }
            Curr.Add(new SuperNode(paths[c], FS));
            return true;
        }

        public static bool Mount(string root, Stream stream)
        {
            return Mount(root, GenericFileSystem.Detect(stream));
        }
    }
}
