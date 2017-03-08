/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
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
        [NoException]
        [Assembly(false)]
        internal static void Switch()
        {
            new Literal("int 0x75");
            new Ret { Offset = 0x0 };
        }

        [NoException]
        [Assembly(false)]
        [Plug("__ISR_Handler_75", Architecture.x86)]
        private static void Handler()
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

            // Load Registers
            new Popad ();

            // Enable Interrupts
            new Sti ();

            // Return
            new Iret ();
        }
    }
}
