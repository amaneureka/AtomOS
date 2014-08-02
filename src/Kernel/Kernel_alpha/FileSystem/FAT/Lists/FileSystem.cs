/* Copyright (C) Atomix OS Development, Inc - All Rights Reserved
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* Written by SANDEEP ILIGER <sandeep.iliger@gmail.com>, 07-2014
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
