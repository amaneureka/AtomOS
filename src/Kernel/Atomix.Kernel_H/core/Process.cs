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
        
        public Process(string aName, UInt32 Directory)
        {
            this.pid = NewID();
            this.AddressSpace = (UInt32*)Directory;
            this.Name = aName;
            this.Threads = new IList<Thread>();
        }

        public void SetEnvironment()
        {
            Paging.CurrentDirectory = AddressSpace;
            Paging.SwitchDirectory((uint)AddressSpace);
        }

        static uint ProcessID = 0;
        static uint NewID()
        {
            return ProcessID++;
        }
    }
}