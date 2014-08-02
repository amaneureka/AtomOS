/* Copyright (C) Atomix OS Development, Inc - All Rights Reserved
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* Written by SANDEEP ILIGER <sandeep.iliger@gmail.com>, 07-2014
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
