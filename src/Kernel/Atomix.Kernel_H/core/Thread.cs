using System;
using System.Runtime.InteropServices;

using Atomix.Kernel_H.arch;
using Atomix.Kernel_H.arch.x86;

namespace Atomix.Kernel_H.core
{
    public class Thread
    {
        public readonly Process Process;
        public Exception Exception;

        ThreadState State;
        uint Address;
        uint StackBottom;
        uint StackTop;
        uint StackLimit;
        
        public Thread(Process Parent, uint xAddress, uint StackStart, uint StackLimit)
        {
            this.Process = Parent;
            this.Address = xAddress;
            this.Process.Threads.Add(this);
            this.State = ThreadState.NotActive;
            this.StackTop = StackStart;
            this.StackBottom = StackTop - StackLimit;
            this.StackLimit = StackLimit;

            if (StackStart != 0)
                SetupInitialStack();
        }

        public void Start()
        {
            if (this.State == ThreadState.NotActive)
            {
                this.State = ThreadState.Running;
                Debug.Write("[@Thread]: Start()\n");
                Scheduler.AddThread(this);
            }
        }

        private unsafe void SetupInitialStack()
        {
            UInt32* Stack = (UInt32*)StackTop;

            // processor data
            *--Stack = 0x202;           // EFLAGS
            *--Stack = 0x08;            // CS
            *--Stack = Address;         // EIP

            // pusha
            *--Stack = 0;               // EDI
            *--Stack = 0;               // ESI
            *--Stack = 0;               // EBP
            *--Stack = 0;               // ESP
            *--Stack = 0;               // EBX
            *--Stack = 0;               // EDX
            *--Stack = 0;               // ECX
            *--Stack = 0;               // EAX

            StackTop = (uint)Stack;
        }

        public uint LoadStack()
        {
            return StackTop;
        }

        public void SaveStack(uint Stack)
        {
            this.StackTop = Stack;
        }

        public void FreeStack()
        {
            Heap.Free(StackBottom, StackLimit);
        }

        public static void Die()
        {
            var curr = Scheduler.CurrentThread;
            if (curr == null)
                return;
            curr.State = ThreadState.Dead;
            while (true) ;//Hook up till the next time slice
        }
                
        public ThreadState Status
        {
            get { return State; }
        }
    }
    public enum ThreadState : int
    {
        NotActive = 0,
        Running = 1,    //Thread is currently running
        Dead = 2,       //Thread is terminated
        Sleep = 3,      //Thread is sleeping
    };
}
