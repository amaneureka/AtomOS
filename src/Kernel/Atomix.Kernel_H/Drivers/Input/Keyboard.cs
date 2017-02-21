/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          PS2 Keyboard driver
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomix.Kernel_H.Core;
using Atomix.Kernel_H.Arch.x86;

namespace Atomix.Kernel_H.Drivers.Input
{
    internal static class Keyboard
    {
        const byte COMMAND = 0x60;

        internal static void Setup()
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
