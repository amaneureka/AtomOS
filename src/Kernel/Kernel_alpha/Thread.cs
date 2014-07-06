using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MT = Kernel_alpha.Multitasking;
using Kernel_alpha.x86.Intrinsic;

namespace Kernel_alpha
{
    public class Thread
    {
        protected int aProcessid;
        protected int SleepCounter;

        #region Constructors
        public int ProcessID
        { get { return aProcessid; } }
        
        public State State
        {
            get 
            {
                return (State)MT.Tasks[aProcessid].state;
            }
            set
            {
                MT.Tasks[aProcessid].state = (ushort)value;
            }
        }
        #endregion

        public Thread(int pid)
        {
            this.aProcessid = pid;
        }

        public void Start()
        {
            this.State = State.Alive;
        }

        /// <summary>
        /// Stop Refrenced Thread
        /// </summary>
        public void Stop()
        {
            this.State = State.Dead;
            IRQ.Timer();
        }

        /// <summary>
        /// Die Current Thread
        /// </summary>
        public static void Die()
        {
            MT.Tasks[MT.CurrentTask].state = (int)State.Dead;
            //As we have done our code, so let other thread to start...So we fire IRQ0
            IRQ.Timer();
        }

        public static void Sleep(uint Cycles)
        {
            MT.Tasks[MT.CurrentTask].state = (int)Cycles;//Cycles should be positive =P
            IRQ.Timer();
        }
    }
}
