using System;

using Atomix.Kernel_H.lib;
using Atomix.Kernel_H.core;
using Atomix.Kernel_H.arch.x86;

namespace Atomix.Kernel_H.core
{
    public static class Scheduler
    {
        static Thread CurrentTask;
        static IQueue<Thread> ThreadQueue;
        
        public static Thread RunningThread
        { get { return CurrentTask; } }

        public static int RunningThreadID
        { 
            get
            {
                return (CurrentTask != null ? CurrentTask.ThreadID : 0);
            }
        }

        public static Process SystemProcess;

        public static void Init()
        {
            ThreadQueue = new IQueue<Thread>(100);
        }
        
        public static void AddThread(Thread th)
        {
            ThreadQueue.Enqueue(th);
        }
        
        public static uint SwitchTask(uint aStack)
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
                        CurrentTask.FreeStack();//Free Stack Memory
                        Heap.Free(CurrentTask);//Free Thread Object
                        break;
                    default://Do nothing for not active
                        break;
                }
            }
            return ThreadQueue.Dequeue();
        }
    }
}
