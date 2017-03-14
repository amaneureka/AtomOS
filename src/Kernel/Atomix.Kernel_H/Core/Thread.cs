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
        internal readonly Process Process;
        internal readonly int ThreadID;

        Action Dead;
        ThreadState State;

        uint mAddress;
        uint* mStackFrame;

        uint mStackTop;
        uint mStackCurrent;
        uint mStackLimit;

        internal ThreadState Status
        {
            get { return State; }
        }

        internal uint StackTop
        { get { return mStackTop; } }

        internal uint StackCurrent
        { get { return mStackCurrent; } }

        static int ThreadCounter;

        public Thread(Process aParent, Action aMethod)
            :this(aParent, aMethod.InvokableAddress(), Heap.kmalloc(0x20000, false) + 0x20000, 0x20000)
        {
        }

        public Thread(Process aParent, Action aMethod, uint aStackStart, uint aStackLimit)
            :this(aParent, aMethod.InvokableAddress(), aStackStart, aStackLimit)
        {
        }

        public Thread(Process aParent, uint aAddress, uint aStackStart, uint aStackLimit, bool SetupStack = true)
        {
            Dead = Die;
            Process = aParent;
            Process.Threads.Add(this);
            State = ThreadState.NotActive;

            mAddress = aAddress;
            mStackTop = aStackStart;
            mStackCurrent = aStackStart;
            mStackFrame = (uint*)(aStackStart - aStackLimit);
            mStackLimit = aStackLimit;

            ThreadID = ++ThreadCounter;

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
            uint* Stack = (uint*)mStackCurrent;

            *--Stack = Dead.InvokableAddress();

            // processor data
            *--Stack = 0x202;           // EFLAGS
            *--Stack = 0x08;            // CS
            *--Stack = mAddress;        // EIP

            // pusha
            *--Stack = 0;               // EDI
            *--Stack = 0;               // ESI
            *--Stack = 0;               // EBP
            *--Stack = 0;               // ESP
            *--Stack = 0;               // EBX
            *--Stack = 0;               // EDX
            *--Stack = 0;               // ECX
            *--Stack = 0;               // EAX

            mStackCurrent = (uint)Stack;
        }

        internal uint LoadStack()
        {
            return mStackCurrent;
        }

        internal void SaveStack(uint Stack)
        {
            mStackCurrent = Stack;
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
