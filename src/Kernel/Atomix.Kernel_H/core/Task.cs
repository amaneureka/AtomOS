/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          thread (task) switch support functions
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomix.Assembler;
using Atomix.Assembler.x86;

using Atomix.Kernel_H.Devices;

using Atomix.CompilerExt.Attributes;

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

        [Assembly, Plug("__ISR_Handler_20")]
        private static void SetupIRQ0()
        {
            // Clear Interrupts
            AssemblyHelper.AssemblerCode.Add(new Cli ());

            // Push all the Registers
            AssemblyHelper.AssemblerCode.Add(new Pushad ());

            // Push ESP
            AssemblyHelper.AssemblerCode.Add(new Push { DestinationReg = Registers.ESP });
            AssemblyHelper.AssemblerCode.Add(new Call ("__Switch_Task__", true));

            // Get New task ESP
            AssemblyHelper.AssemblerCode.Add(new Pop { DestinationReg = Registers.ESP });

            // Tell CPU that we have recieved IRQ
            AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.AL, SourceRef = "0x20", Size = 8 });
            AssemblyHelper.AssemblerCode.Add(new Out { DestinationRef = "0x20", SourceReg = Registers.AL });

            // Load Registers
            AssemblyHelper.AssemblerCode.Add(new Popad ());

            // Enable Interrupts
            AssemblyHelper.AssemblerCode.Add(new Sti ());

            AssemblyHelper.AssemblerCode.Add(new Iret ());
        }
    }
}
