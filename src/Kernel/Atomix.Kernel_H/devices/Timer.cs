/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          System Hardware Timer (IRQ0)
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomix.Kernel_H.Core;
using Atomix.Kernel_H.Arch.x86;

namespace Atomix.Kernel_H.devices
{
    public static class Timer
    {
        public static void Setup()
        {
            Debug.Write("Initializing interval timer\n");

            /* Set Timer Frequency to 100Hz */
            SetFrequency(100);
        }

        public static uint TicksFromStart
        {
            get;
            private set;
        }

        public static void Tick()
        {
            // Tick Tok Tick Tok :P
            TicksFromStart++;
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
    }
}
