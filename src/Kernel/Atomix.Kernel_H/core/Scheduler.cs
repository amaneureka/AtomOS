using System;

using Atomix.Kernel_H.lib;
using Atomix.Kernel_H.core;
using Atomix.Kernel_H.arch.x86;

namespace Atomix.Kernel_H.core
{
    public static class Scheduler
    {
        private static IQueue<Thread> ThreadQueue;
        private static Thread CurrentTask;
        
        public static void Init()
        {
            ThreadQueue = new IQueue<Thread>();
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

#warning develop this spin lock
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
