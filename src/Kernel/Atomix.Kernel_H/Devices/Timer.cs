/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          System Hardware Timer (IRQ0)
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomixilc;
using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;

using Atomix.Kernel_H.Core;
using Atomix.Kernel_H.Arch.x86;

namespace Atomix.Kernel_H.Devices
{
    internal static class Timer
    {
        internal static uint TicksFromStart;

        internal static void Setup()
        {
            Debug.Write("Initializing interval timer\n");

            /* Set Timer Frequency to 100Hz */
            SetFrequency(100);
        }

        [Label("__Timer_Handler__")]
        private static uint Handler(uint aOldStack)
        {
            TicksFromStart++;
            return Scheduler.SwitchTask(aOldStack);
        }

        private static void SetFrequency(int Hz)
        {
            int divisor = 1193180 / Hz;
            PortIO.Out8(0x43, 0x36);                            /* Set our command byte 0x36 */
            PortIO.Out8(0x40, (byte)(divisor & 0xFF));          /* Set low byte of divisor */
            PortIO.Out8(0x40, (byte)(divisor >> 8));            /* Set high byte of divisor */

            /* Enable Timer IRQ (Clear mask) */
            byte value = (byte)(PortIO.In8(0x21) & 0xFE);
            PortIO.Out8(0x21, value);
        }

        [NoException]
        [Assembly(false)]
        [Plug("__ISR_Handler_20", Architecture.x86)]
        private static void IRQ0()
        {
            // Clear Interrupts
            new Cli();

            // Push all the Registers
            new Pushad();

            // Tell CPU that we have recieved IRQ
            new Mov { DestinationReg = Register.AL, SourceRef = "0x20", Size = 8 };
            new Out { DestinationRef = "0x20", SourceReg = Register.AL };

            // Push ESP
            new Push { DestinationReg = Register.ESP };
            new Call { DestinationRef = "__Timer_Handler__", IsLabel = true };

            // Get New task ESP
            new Mov { DestinationReg = Register.ESP, SourceReg = Register.EAX };

            // Load Registers
            new Popad();

            // Enable Interrupts
            new Sti();

            new Iret();
        }
    }
}
