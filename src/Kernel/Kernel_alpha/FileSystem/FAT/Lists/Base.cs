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
    public abstract class Base
    {
        protected string Name;
        protected Details Details;

        public string EntryName
        { get { return Name; } }

        public Details EntryDetails
        { get { return Details; } }

        public Base(string aName, Details aDetail)
        {
            this.Name = aName;
            this.Details = aDetail;
        }
    }
}
