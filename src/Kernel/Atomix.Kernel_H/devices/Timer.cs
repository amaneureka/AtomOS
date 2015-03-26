using System;
using System.Collections.Generic;

using Atomix.Kernel_H.core;
using Atomix.Kernel_H.arch.x86;

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

        private static uint ElapsedSeconds = 0;
        private static uint ElapsedMiliSeconds = 0;

        public static void Tick()
        {
            if (++ElapsedMiliSeconds == 100)
            {
                ElapsedMiliSeconds = 0;
                ElapsedSeconds++;
            }
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
