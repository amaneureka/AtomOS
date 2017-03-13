/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Thread Class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomixilc.Lib;

namespace Atomix.Kernel_H.Core
{
    internal unsafe class Thread
    {
        internal readonly GC GC;
        internal readonly Process Process;
        internal readonly int ThreadID;

        Action Dead;
        ThreadState State;

        uint Address;

        uint* StackTop;
        uint StackLimit;

        internal ThreadState Status
        {
            get { return State; }
        }

        static int ThreadCounter;

        public Thread(Process aParent, Action aMethod)
            :this(aParent, aMethod.InvokableAddress(), Heap.kmalloc(0x20000) + 0x20000, 0x20000)
        {
            return;
        }

        public Thread(Process aParent, Action aMethod, uint aStackStart, uint aStackLimit)
            :this(aParent, aMethod.InvokableAddress(), aStackStart, aStackLimit)
        {
            return;
        }

        public Thread(Process aParent, uint aAddress, uint aStackStart, uint aStackLimit, bool SetupStack = true)
        {
            Dead = Die;
            Process = aParent;
            Address = aAddress;
            Process.Threads.Add(this);
            State = ThreadState.NotActive;
            StackTop = (uint*)aStackStart;
            StackLimit = aStackLimit;
            ThreadID = ++ThreadCounter;
            GC = new GC(aStackStart);

            if (SetupStack)
                SetupInitialStack();
        }

        internal void Start()
        {
            if (State == ThreadState.NotActive)
            {
                State = ThreadState.Running;
                Debug.Write("[Thread]: Start()\n");
                Scheduler.AddThread(this);
            }
        }

        private unsafe void SetupInitialStack()
        {
            uint* Stack = (uint*)StackTop;

            *--Stack = Dead.InvokableAddress();

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

            StackTop = Stack;
        }

        internal uint LoadStack()
        {
            return (uint)StackTop;
        }

        internal void SaveStack(uint Stack)
        {
            StackTop = (uint*)Stack;
        }

        internal static void Die()
        {
            var curr = Scheduler.RunningThread;
            if (curr == null)
                return;

            curr.State = ThreadState.Dead;
            Debug.Write("Thread.Die() : %d\n", (uint)curr.ThreadID);
            while (true) ;// Hook up till the next time slice
        }
    }

    internal enum ThreadState : int
    {
        NotActive = 0,
        Running = 1,    // Thread is currently running
        Dead = 2,       // Thread is terminated
        Sleep = 3,      // Thread is sleeping
    };
}
