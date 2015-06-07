using System;

using Atomix.Kernel_H.core;
using Atomix.Kernel_H.arch.x86;

namespace Atomix.Kernel_H.drivers.input
{
    public static class Keyboard
    {
        const byte COMMAND = 0x60;

        public static void Setup()
        {
            Debug.Write("PS/2 Keyboard Controller Setup\n");
            IDT.RegisterInterrupt(HandleIRQ, 0x21);
        }

        private static void HandleIRQ(ref IRQContext context)
        {
            uint xScanCode = PortIO.In8(COMMAND);
        }
    }
}
