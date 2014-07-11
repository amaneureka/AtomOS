using System;
using System.Collections.Generic;

using Atomix.Assembler;
using Atomix.Assembler.x86;
using Atomix.CompilerExt.Attributes;
using asm = Atomix.Assembler.AssemblyHelper;

using libAtomixH.Core;
using libAtomixH.IO.Ports;

namespace libAtomixH.Threading
{
    public static class Scheduler
    {
        private static Task[] mTasks = new Task[255];
        private static int mCurrentTask = -1;
        private static int Counter = 0;

        private static Queue<int> RunningTasks = new Queue<int> ();

        public static Task[] Tasks
        {
            get { return mTasks; }
        }

        public static int CurrentTask
        {
            get { return mCurrentTask; }
        }

        public static void Init ()
        {
            uint divisor = (1193180 * 10) / 1000;               /* Calculate our divisor */ //10ms
            Native.Out8 (0x43, 0x36);                            /* Set our command byte 0x36 */
            Native.Out8 (0x40, (byte)(divisor & 0xFF));          /* Set low byte of divisor */
            Native.Out8 (0x40, (byte)(divisor >> 8));            /* Set high byte of divisor */

            mCurrentTask = 0;
            PIC.ClearMask (0);
        }

        [Plug ("__Task_Switcher__")]
        public static uint TaskSwitcher (uint context)
        {
            // save the old context into current task
            mTasks[mCurrentTask].Stack = context;

            //Update the sleeping tasks
            UpdateHaltedTasks ();

            //Get Task to Run
            mCurrentTask = GetTaskToRun ();

            // Return new task's context.
            return mTasks[mCurrentTask].Stack;
        }

        public static int GetTaskToRun ()
        {
            var xState = mTasks[mCurrentTask].state;
            switch ((ThreadState)xState)
            {
                case ThreadState.Alive:
                    RunningTasks.Enqueue (mCurrentTask);
                    break;
                case ThreadState.Dead:
                case ThreadState.None:
                    break;
            }

            return RunningTasks.Dequeue ();
        }

        public static void UpdateHaltedTasks ()
        {
            int xState;

            /* Sleeping Tasks */
            for (int i = 0; i < Counter; i++)
            {
                xState = mTasks[i].state;
                if (xState > 0)
                {
                    mTasks[i].state--;
                    if (xState == 1)
                        RunningTasks.Enqueue (i);
                }
            }
        }

        public static unsafe Thread CreateTask (uint xLoc, bool isRunning)
        {
            Native.ClearInterrupt ();

            uint* stack;

            Task* task = (Task*)(Heap.AllocateMem ((uint)sizeof (Task)));
            task->Stack = Heap.AllocateMem (0x1000) + 0x1000;//Put pointer at end

            stack = (uint*)task->Stack;

            // Expand down stack
            // processor data
            *--stack = 0x202;       // EFLAGS
            *--stack = 0x08;        // CS
            *--stack = xLoc;        // EIP

            // pusha
            *--stack = 0;           // EDI
            *--stack = 0;           // ESI
            *--stack = 0;           // EBP
            *--stack = 0;           // NULL
            *--stack = 0;           // EBX
            *--stack = 0;           // EDX
            *--stack = 0;           // ECX
            *--stack = 0;           // EAX

            // data segments
            *--stack = 0x10;        // DS
            *--stack = 0x10;        // ES
            *--stack = 0x10;        // FS
            *--stack = 0x10;        // GS

            task->Stack = (uint)stack;
            task->Address = (uint*)xLoc;
            task->state = (byte)(isRunning ? ThreadState.Alive : ThreadState.None);

            mTasks[Counter++] = *task;

            if (isRunning)
                RunningTasks.Enqueue (Counter - 1);

            Native.SetInterrupt ();
            return new Thread (Counter - 1);
        }

        [Assembly, Plug ("__ISR_Handler_20")]
        private static void SetupIRQ0 ()
        {
            //We setup IRQ0 here =)
            asm.AssemblerCode.Add (new Pushad ());
            asm.AssemblerCode.Add (new Push { DestinationReg = Registers.DS, Size = 16 });
            asm.AssemblerCode.Add (new Push { DestinationReg = Registers.ES, Size = 16 });
            asm.AssemblerCode.Add (new Push { DestinationReg = Registers.FS, Size = 16 });
            asm.AssemblerCode.Add (new Push { DestinationReg = Registers.GS, Size = 16 });

            asm.AssemblerCode.Add (new Mov { DestinationReg = Registers.AX, SourceRef = "0x10", Size = 16 });
            asm.AssemblerCode.Add (new Mov { DestinationReg = Registers.DS, SourceReg = Registers.AX, Size = 16 });
            asm.AssemblerCode.Add (new Mov { DestinationReg = Registers.ES, SourceReg = Registers.AX, Size = 16 });
            asm.AssemblerCode.Add (new Mov { DestinationReg = Registers.FS, SourceReg = Registers.AX, Size = 16 });
            asm.AssemblerCode.Add (new Mov { DestinationReg = Registers.GS, SourceReg = Registers.AX, Size = 16 });
            asm.AssemblerCode.Add (new Mov { DestinationReg = Registers.SS, SourceReg = Registers.AX, Size = 16 });

            asm.AssemblerCode.Add (new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.ESP });
            asm.AssemblerCode.Add (new Push { DestinationReg = Registers.EAX });

            asm.AssemblerCode.Add (new Call ("__Task_Switcher__"));
            asm.AssemblerCode.Add (new Mov { DestinationReg = Registers.ESP, SourceReg = Registers.EAX });

            asm.AssemblerCode.Add (new Mov { DestinationReg = Registers.AL, SourceRef = "0x20", Size = 8 });
            asm.AssemblerCode.Add (new Out { DestinationRef = "0x20", SourceReg = Registers.AL });

            asm.AssemblerCode.Add (new Pop { DestinationReg = Registers.GS, Size = 16 });
            asm.AssemblerCode.Add (new Pop { DestinationReg = Registers.FS, Size = 16 });
            asm.AssemblerCode.Add (new Pop { DestinationReg = Registers.ES, Size = 16 });
            asm.AssemblerCode.Add (new Pop { DestinationReg = Registers.DS, Size = 16 });
            asm.AssemblerCode.Add (new Popad ());
            asm.AssemblerCode.Add (new Iret ());
        }
    }
}
