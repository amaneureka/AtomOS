/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          thread (task) switch support functions
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomixilc;
using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;

using Atomix.Kernel_H.Devices;

namespace Atomix.Kernel_H.Core
{
    internal static class Task
    {
        [Label("__Switch_Task__")]
        internal static uint SwitchTask(uint oldStack)
        {
            // Increment System Timer
            Timer.Tick();
            return Scheduler.SwitchTask(oldStack);
        }

        [NoException]
        [Assembly(false)]
        [Plug("__ISR_Handler_20", Architecture.x86)]
        private static void SetupIRQ0()
        {
            // Clear Interrupts
            new Cli ();

            // Push all the Registers
            new Pushad ();

            // Push ESP
            new Push { DestinationReg = Register.ESP };
            new Call { DestinationRef = "__Switch_Task__", IsLabel = true };

            // Get New task ESP
            new Mov { DestinationReg = Register.ESP, SourceReg = Register.EAX };

            // Tell CPU that we have recieved IRQ
            new Mov { DestinationReg = Register.AL, SourceRef = "0x20", Size = 8 };
            new Out { DestinationRef = "0x20", SourceReg = Register.AL };

            // Load Registers
            new Popad ();

            // Enable Interrupts
            new Sti ();

            new Iret ();
        }
    }
}
