/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          File Contains various mscorlib plug belongs to Threading class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomixilc;
using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;

namespace Atomix.Kernel_H.plugs
{
    internal class ThreadingImpl
    {
        [Assembly(true), Plug("System_Void_System_Threading_Monitor_Enter_System_Object__System_Boolean__")]
        internal static void AcquireLock(object aObj, ref bool aLockTaken)
        {
            new Call { DestinationRef = "AcquireLock", IsLabel = true };
            // we don't carry exception flag from here, although Thread.Exception contains the flag
        }

        [Assembly(true), Plug("System_Void_System_Threading_Monitor_Exit_System_Object_")]
        internal static void ReleaseLock(object aObj)
        {
            new Call { DestinationRef = "ReleaseLock", IsLabel = true };
            // we don't carry exception flag from here, although Thread.Exception contains the flag
        }
    }
}
