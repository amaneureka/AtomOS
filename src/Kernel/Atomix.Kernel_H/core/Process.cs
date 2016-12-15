/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Process Class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomixilc.Lib;

using Atomix.Kernel_H.Lib;
using Atomix.Kernel_H.Arch.x86;

namespace Atomix.Kernel_H.Core
{
    internal unsafe class Process
    {
        internal readonly uint pid;
        internal readonly string Name;
        internal readonly IList<Thread> Threads;
        internal readonly uint[] shm_mapping;// Using Bitmask keeps a track of which shm region is occupied

        uint mPageDirectory;
        IDictionary<string, uint> mSymbols;

        internal Process(string aName, UInt32 Directory)
        {
            pid = NewID();
            mPageDirectory = Directory;
            Name = aName;
            Threads = new IList<Thread>();
            shm_mapping = new uint[SHM.LIMIT_TO_PROCESS];
            mSymbols = new IDictionary<string, uint>(Internals.GetHashCode, string.Equals);
        }

        internal void SetEnvironment()
        {
            Paging.SwitchDirectory(mPageDirectory);
        }

        internal uint GetSymbols(string aStr)
        {
            return mSymbols.GetValue(aStr, 0);
        }

        internal void SetSymbol(string aStr, uint aAddress)
        {
            uint add = mSymbols.GetValue(aStr, 0);
            if (add != 0)
                throw new Exception("[Process]: Symbol already exist!");
            mSymbols.Add(aStr, aAddress);
        }

        static uint ProcessID = 0;
        static uint NewID()
        {
            return ProcessID++;
        }
    }
}