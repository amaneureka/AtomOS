using System;
using System.Collections.Generic;

using Atomix.Kernel_H.core;
using Atomix.Kernel_H.arch.x86;

namespace Atomix.Kernel_H.core
{
    public static class Scheduler
    {
        private static Queue<Thread> ThreadQueue;
        private static Thread CurrentTask;
        
        public static void Init()
        {
            ThreadQueue = new Queue<Thread>();
        }
        
        public static void AddThread(Thread th)
        {
            ThreadQueue.Enqueue(th);
        }

        public static uint SwitchTask(uint aStack)
        {
            if (IsLocked)
                return aStack;

            var NextTask = InvokeNext();

            if (CurrentTask == null)
            {
                CurrentTask = NextTask;
                return aStack;
            }
            
            if ((CurrentTask == NextTask) || (NextTask == null))
            {
                return aStack;
            }
            
            CurrentTask.SaveStack(aStack);            
            if (NextTask.Process != CurrentTask.Process)
                NextTask.Process.SetEnvironment();
            CurrentTask = NextTask;
            return NextTask.LoadStack();
        }

        static bool IsLocked = false;
        public static void SpinLock(bool status)
        {
            IsLocked = status;
        }

        private static Thread InvokeNext()
        {
            if (ThreadQueue.Count == 0)
                return null;

            if (CurrentTask != null)
                ThreadQueue.Enqueue(CurrentTask);
            return ThreadQueue.Dequeue();
        }
    }
}
