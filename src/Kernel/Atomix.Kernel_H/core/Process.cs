using System;

using Atomix.Kernel_H.lib;
using Atomix.Kernel_H.arch.x86;

namespace Atomix.Kernel_H.core
{
    public unsafe class Process
    {
        public readonly uint pid;
        public readonly string Name;
        public readonly UInt32* AddressSpace;
        public IList<Thread> Threads;
        public uint[] shm_mapping;//Using Bitmask keeps a track of which shm region is occupied
        
        public Process(string aName, UInt32 Directory)
        {
            this.pid = NewID();
            this.AddressSpace = (UInt32*)Directory;
            this.Name = aName;
            this.Threads = new IList<Thread>();
#warning Maximum Number of Frames to shm mapping is a random constant
            this.shm_mapping = new uint[SHM.LIMIT_TO_PROCESS];
        }

        public void SetEnvironment()
        {
            Paging.SwitchDirectory((uint)AddressSpace);
        }

        static uint ProcessID = 0;
        static uint NewID()
        {
            return ProcessID++;
        }
    }
}