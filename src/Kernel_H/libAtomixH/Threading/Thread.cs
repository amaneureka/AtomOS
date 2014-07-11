using System;

using libAtomixH.Core;

namespace libAtomixH.Threading
{
    public class Thread
    {
        protected int aProcessid;
        protected int SleepCounter;

        public int ProcessID
        { get { return aProcessid; } }
        
        public ThreadState State
        {
            get 
            {
                return (ThreadState)Scheduler.Tasks[aProcessid].state;
            }
            set
            {
                Scheduler.Tasks[aProcessid].state = (ushort)value;
            }
        }

        public Thread(int pid)
        {
            this.aProcessid = pid;
        }

        public void Start()
        {
            this.State = ThreadState.Alive;
        }

        /// <summary>
        /// Stop Referenced Thread
        /// </summary>
        public void Stop()
        {
            this.State = ThreadState.Dead;
            IRQ.Timer();
        }

        /// <summary>
        /// Kill the current Thread
        /// </summary>
        public static void Die()
        {
            Scheduler.Tasks[Scheduler.CurrentTask].state = (int)ThreadState.Dead;
            //As we have done our code, so let other thread to start...So we fire IRQ0
            IRQ.Timer();
        }

        /// <summary>
        /// Let the current Thread sleep for <paramref name="n" /> cycles
        /// </summary>
        /// <param name="n">The cycle count</param>
        public static void Sleep(uint n)
        {
            // Cycles should be positive
            Scheduler.Tasks[Scheduler.CurrentTask].state = (int)n;
            IRQ.Timer();
        }
    }
}
