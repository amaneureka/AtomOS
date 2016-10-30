/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Thread Class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomix.Kernel_H.Lib;

namespace Atomix.Kernel_H.Core
{
    internal class Thread
    {
        internal readonly Process Process;
        internal readonly int ThreadID;
        internal Exception Exception;

        ThreadState State;
        uint Address;
        uint StackBottom;
        uint StackTop;
        uint StackLimit;

        static int ThreadCounter = 0;

        public Thread(Process aParent, Action aMethod, uint aStackStart, uint aStackLimit)
            :this(aParent, aMethod.GetAddress(), aStackStart, aStackLimit)
        {
            return;
        }

        public Thread(Process aParent, uint aAddress, uint aStackStart, uint aStackLimit)
        {
            Process = aParent;
            Address = aAddress;
            Process.Threads.Add(this);
            State = ThreadState.NotActive;
            StackTop = aStackStart;
            StackBottom = aStackStart - aStackLimit;
            StackLimit = aStackLimit;
            ThreadID = ++ThreadCounter;

            if (aStackStart != 0)
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

        internal uint LoadStack()
        {
            return StackTop;
        }

        internal void SaveStack(uint Stack)
        {
            this.StackTop = Stack;
        }

        internal void FreeStack()
        {
            Heap.Free(StackBottom, StackLimit);
        }

        internal static void Die()
        {
            var curr = Scheduler.RunningThread;
            if (curr == null)
                return;
            curr.State = ThreadState.Dead;
            while (true) ;// Hook up till the next time slice
        }

        internal ThreadState Status
        {
            get { return State; }
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
