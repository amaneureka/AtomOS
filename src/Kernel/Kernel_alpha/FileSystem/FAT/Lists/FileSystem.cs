/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:
* PROGRAMMERS:      SANDEEP ILIGER <sandeep.iliger@gmail.com>
*                   Aman Priyadarshi <aman.eureka@gmail.com>
*/

using System;
using System.Collections.Generic;

namespace Kernel_alpha.FileSystem.FAT.Lists
{
    public class FileSystem
    {
        static protected FileSystem mFS;

        static public void AddMapping(string aPath, FileSystem aFileSystem)
        {
            //mMappings.Add(aPath.ToUpper(), aFileSystem);
            // Dictionary<> doesnt work yet, so for now we just hack this and support only one FS
            mFS = aFileSystem;
        }
    }
}
