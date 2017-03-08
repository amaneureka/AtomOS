/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:
* PROGRAMMERS:      SANDEEP ILIGER <sandeep.iliger@gmail.com>
*                   Aman Priyadarshi <aman.eureka@gmail.com>
*/

using System;

namespace Kernel_alpha.FileSystem.FAT.Lists
{
    public class Directory : Base
    {
        public Directory(string aName, Details aDetail)
            : base(aName, aDetail)
        {
        }
    }
}
