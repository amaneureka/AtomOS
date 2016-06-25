﻿/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Kernel Thread Scheduler
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomix.Kernel_H.lib;

namespace Atomix.Kernel_H.core
{
    internal static class Scheduler
    {
        static Thread CurrentTask;
        static IQueue<Thread> ThreadQueue;

        internal static Process RunningProcess
        {
            get
            {
                if (CurrentTask != null)
                    return CurrentTask.Process;
                return SystemProcess;
            }
        }

        internal static Thread RunningThread
        { get { return CurrentTask; } }

        internal static int RunningThreadID
        { 
            get
            {
                return (CurrentTask != null ? CurrentTask.ThreadID : 0);
            }
        }

        internal static Process SystemProcess;

        internal static void Init()
        {
            ThreadQueue = new IQueue<Thread>(100);
        }

        internal static void AddThread(Thread th)
        {
            ThreadQueue.Enqueue(th);
        }

        internal static uint SwitchTask(uint aStack)
        {
            var NextTask = InvokeNext();

            if (CurrentTask == null)
            {
                CurrentTask = NextTask;
                return aStack;
            }
            
            if ((CurrentTask == NextTask) || (NextTask == null))
                return aStack;
            
            CurrentTask.SaveStack(aStack);            
            if (NextTask.Process != CurrentTask.Process)
                NextTask.Process.SetEnvironment();
            CurrentTask = NextTask;
            return NextTask.LoadStack();
        }
                
        private static Thread InvokeNext()
        {
            if (ThreadQueue.Count == 0)
                return null;

            if (CurrentTask != null)
            {
                var state = CurrentTask.Status;
                switch (state)
                {
                    case ThreadState.Running:
                        ThreadQueue.Enqueue(CurrentTask);
                        break;
                    case ThreadState.Dead:
                        CurrentTask.FreeStack();// Free Stack Memory
                        Heap.Free(CurrentTask);// Free Thread Object
                        break;
                    default:// Do nothing for not active
                        break;
                }
            }
            return ThreadQueue.Dequeue();
        }
    }
}
