/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Process Class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomix.Kernel_H.lib;
using Atomix.Kernel_H.arch.x86;
using Atomix.Kernel_H.lib.crypto;

namespace Atomix.Kernel_H.core
{
    public unsafe class Process
    {
        public readonly uint pid;
        public readonly string Name;
        public readonly IList<Thread> Threads;
        public readonly uint[] shm_mapping;//Using Bitmask keeps a track of which shm region is occupied

        uint mPageDirectory;
        IDictionary<string, uint> mSymbols;
        
        public Process(string aName, UInt32 Directory)
        {
            pid = NewID();
            mPageDirectory = Directory;
            Name = aName;
            Threads = new IList<Thread>();
            shm_mapping = new uint[SHM.LIMIT_TO_PROCESS];
            mSymbols = new IDictionary<string, uint>(sdbm.GetHashCode, string.Equals);
        }
        
        public void SetEnvironment()
        {
            Paging.SwitchDirectory(mPageDirectory);
        }

        public uint GetSymbols(string aStr)
        {
            return mSymbols.GetValue(aStr, 0);
        }

        public void SetSymbol(string aStr, uint aAddress)
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