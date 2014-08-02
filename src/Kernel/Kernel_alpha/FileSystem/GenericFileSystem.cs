/* Copyright (C) Atomix OS Development, Inc - All Rights Reserved
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* Written by SANDEEP ILIGER <sandeep.iliger@gmail.com>, 07-2014
*/

using System;
using System.Collections.Generic;
using Kernel_alpha.Drivers;
using Kernel_alpha.FileSystem.FAT;

namespace Kernel_alpha.FileSystem
{
    public abstract class GenericFileSystem
    {
        protected BlockDevice IDevice;
        protected bool mIsValid;

        public BlockDevice Device
        { get { return IDevice; } }

        public bool IsValid
        { get { return mIsValid; } }


    }

    public abstract class ACompare
    {
        public abstract bool Compare(byte[] data, uint offset, FatType type);
       
    }
}
