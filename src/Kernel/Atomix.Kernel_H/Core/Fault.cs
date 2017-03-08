/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          ISR Fault Extension
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomixilc.Lib;

using Atomix.Kernel_H.Arch.x86;

namespace Atomix.Kernel_H.Core
{
    internal static class Fault
    {
        /// <summary>
        /// System at critical condition, so crash the system by giving exception
        /// </summary>
        internal static void Handle(ref IRQContext aDump)
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
            Debug.Write("CR2: %d\n", Native.CR2Register());
            Debug.Write("Thread-ID: %d\n", Scheduler.RunningThread.ThreadID);
            Native.Cli();
            Native.Hlt();
        }
    }
}
