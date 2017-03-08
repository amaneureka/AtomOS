/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Apply a mutual exclusive lock to thread also implements `lock`keyword
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomixilc.Lib;

namespace Atomix.Kernel_H.Core
{
    internal static class Monitor
    {
        internal static void AcquireLock(ref uint aLock)
        {
            while (Native.AtomicExchange(ref aLock, 1) != 0)
                Task.Switch();
        }

        internal static void ReleaseLock(ref uint aLock)
        {
            aLock = 0;
        }
    }
}
