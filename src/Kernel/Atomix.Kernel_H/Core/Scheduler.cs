/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Kernel Thread Scheduler
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomixilc;
using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;

using Atomix.Kernel_H.Lib;
using Atomix.Kernel_H.Arch.x86;

namespace Atomix.Kernel_H.Core
{
    internal static class Scheduler
    {
        static Thread CurrentTask;
        static IQueue<Thread> ThreadQueue;

        internal static Process SystemProcess;

        internal static Process RunningProcess
        {
            get { return CurrentTask.Process; }
        }

        internal static Thread RunningThread
        {
            get { return CurrentTask; }
        }

        internal static void Init(uint KernelDirectory)
        {
            ThreadQueue = new IQueue<Thread>(100);

            SystemProcess = new Process("System", KernelDirectory);
            CurrentTask = new Thread(SystemProcess, 0, 0, 10000);

            CurrentTask.Start();
        }

        internal static void AddThread(Thread th)
        {
            ThreadQueue.Enqueue(th);
        }

        [Label("__Switch_Task__")]
        internal static uint SwitchTask(uint aStack)
        {
            var NextTask = InvokeNext();

            if (CurrentTask == NextTask)
                return aStack;

            CurrentTask.SaveStack(aStack);
            if (NextTask.Process != CurrentTask.Process)
                NextTask.Process.SetEnvironment();

            CurrentTask = NextTask;
            return NextTask.LoadStack();
        }

        private static Thread InvokeNext()
        {
            var state = CurrentTask.Status;
            switch (state)
            {
                case ThreadState.Running:
                    ThreadQueue.Enqueue(CurrentTask);
                    break;
                case ThreadState.Dead:
                    CurrentTask.Free();
                    break;
                default:// Do nothing for not active
                    break;
            }

            return ThreadQueue.Dequeue();
        }
    }
}
