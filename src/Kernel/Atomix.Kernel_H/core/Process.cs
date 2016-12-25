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
    internal class Process
    {
        internal readonly uint ID;
        internal readonly string Name;
        internal readonly uint PageDirectory;
        internal readonly IList<Thread> Threads;
        internal readonly IDictionary<string, uint> Symbols;
        internal readonly uint[] shm_mapping;

        internal uint HeapCurrent;
        internal uint HeapEndAddress;
        internal readonly uint HeapStartAddress;

        internal Process(string aName, uint aDirectory)
        {
            Name = aName;
            ID = GetNewProcessID();
            PageDirectory = aDirectory;

            Threads = new IList<Thread>();
            Symbols = new IDictionary<string, uint>(Internals.GetHashCode, string.Equals);

            // TODO: Should be a random address
            HeapStartAddress = 0xA0000000;
            HeapCurrent = HeapStartAddress;
            HeapEndAddress = HeapStartAddress;

            shm_mapping = new uint[SHM.LIMIT_TO_PROCESS];
        }

        internal void SetEnvironment()
        {
            Paging.SwitchDirectory(PageDirectory);
        }

        internal uint GetSymbols(string aStr)
        {
            return Symbols.GetValue(aStr, 0);
        }

        internal void SetSymbol(string aStr, uint aAddress)
        {
            uint add = Symbols.GetValue(aStr, 0);
            if (add != 0)
                throw new Exception("[Process]: Symbol already exist!");
            Symbols.Add(aStr, aAddress);
        }

        static uint _pid = 0;
        static uint GetNewProcessID()
        {
            return _pid++;
        }
    }
}