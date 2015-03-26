using System;
using System.Collections.Generic;

using Atomix.Kernel_H.arch.x86;

namespace Atomix.Kernel_H.core
{
    public static class Fault
    {
        /// <summary>
        /// System at critical condition, so crash the system by giving exception
        /// </summary>
        public static void Handle(ref IRQContext aDump)
        {
            Debug.Write("Unhandled Interrupt: %d\nStack Dump::\n", aDump.Interrupt);
            Debug.Write("Error Code: %d\n", aDump.ErrorCode);
            Debug.Write("EIP: %d\n", aDump.EIP);
            Debug.Write("CS: %d\n", aDump.CS);
            Debug.Write("EBP: %d\n", aDump.EBP);
            Debug.Write("ESP: %d\n", aDump.ESP);
            Debug.Write("EAX: %d\n", aDump.EAX);
            Debug.Write("EBX: %d\n", aDump.EBX);
            Debug.Write("ECX: %d\n", aDump.ECX);
            Debug.Write("EDX: %d\n", aDump.EDX);
            Debug.Write("EDI: %d\n", aDump.EDI);
            Debug.Write("ESI: %d\n", aDump.ESI);
            Native.Hlt();
        }
    }
}
