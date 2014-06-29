using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                return (State)Multitasking.Tasks[aProcessid].state;
            }
            set
            {
                Multitasking.Tasks[aProcessid].state = (ushort)value;
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

        public void Stop()
        {
            this.State = State.Dead;
        }
    }
}
