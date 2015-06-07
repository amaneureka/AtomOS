using System;

using Atomix.Kernel_H.lib;
using Atomix.Kernel_H.core;
using Atomix.Kernel_H.arch.x86;

namespace Atomix.Kernel_H.core
{
    public static class Scheduler
    {
        private static IQueue<Thread> ThreadQueue;
        private static IList<Thread> SleepingThreadsQueue;
        private static Thread CurrentTask;
        private static byte[] ResourceArray = new byte[100];

        public static Thread CurrentThread
        { get { return CurrentTask; } }

        public static void Init()
        {
            ThreadQueue = new IQueue<Thread>(100);//Allocate memory for atleast 100 threads, later on increase it
            SleepingThreadsQueue = new IList<Thread>(100);
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
            {
                return aStack;
            }
            
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
            while (ResourceArray[ID] != 0) ;//Hookup that thread till other thread free up that resource
            ResourceArray[ID] = 1;
        }

        /// <summary>
        /// Free up the resource
        /// </summary>
        /// <param name="ID"></param>
        public static void SpinUnlock(int ID)
        {
            ResourceArray[ID] = 0;
        }

        static int ResID = 0;
        /// <summary>
        /// Get Resource ID for spinlock
        /// </summary>
        /// <returns></returns>
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
                    case ThreadState.Sleep:
                        SleepingThreadsQueue.Add(CurrentTask);
                        break;
                    case ThreadState.Dead:
                        CurrentTask.FreeStack();//Free Stack Memory
                        Heap.Free(CurrentTask);//Free Thread Object
                        break;
                    default://Do nothing for not active
                        break;
                }                
            }
            //Update Sleeping threads
            for (int i = 0; i < SleepingThreadsQueue.Count; i++)
            {
                var th = SleepingThreadsQueue[i];
                if (--th.SleepTicks == 0)
                {
                    ThreadQueue.Enqueue(th);
                    th.WakeUp();
                    SleepingThreadsQueue[i] = null;
                }
            }
            SleepingThreadsQueue.Refresh();
            return ThreadQueue.Dequeue();
        }
    }
}
