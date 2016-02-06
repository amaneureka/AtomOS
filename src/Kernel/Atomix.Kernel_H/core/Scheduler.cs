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

        static byte[] ResourceArray = new byte[500];

        public static Thread CurrentThread
        { get { return CurrentTask; } }

        public static Process CurrentProcess
        { get { return CurrentTask.Process; } }

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

        /// <summary>
        /// Lock up this resource till it won't freed up
        /// </summary>
        /// <param name="ID"></param>
        public static void SpinLock(int ID)
        {
            //we should switch task here because we are running on single core
            while (ResourceArray[ID] != 0) ;//Hookup that thread till other thread free up that resource
            ResourceArray[ID] = 1;
        }

        public static void SpinUnlock(int ID)
        {
            ResourceArray[ID] = 0;
        }

        static int ResID = 0;
        public static int GetResourceID()
        {
            return ResID++;
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
                        Heap.Free(CurrentTask);//Free Thread Object
                        CurrentTask.FreeStack();//Free Stack Memory
                        break;
                    default://Do nothing for not active
                        break;
                }
            }
            return ThreadQueue.Dequeue();
        }
    }
}
