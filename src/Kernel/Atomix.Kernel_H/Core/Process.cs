/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Process Class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomixilc.Lib;

using Atomix.Kernel_H.IO;
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
        internal readonly uint[] shm_mapping;
        internal readonly IList<Stream> Files;

        internal uint HeapCurrent;
        internal uint HeapEndAddress;
        internal readonly uint HeapStartAddress;

        internal Process(string aName, uint aDirectory)
        {
            Name = aName;
            ID = GetNewProcessID();
            PageDirectory = aDirectory;

            Files = new IList<Stream>();
            Threads = new IList<Thread>();

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

        static uint _pid = 0;
        static uint GetNewProcessID()
        {
            return _pid++;
        }
    }
}