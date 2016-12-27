/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
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
            while (Native.AtomicExchange(ref aLock, 1) != 0) ;
        }

        internal static void ReleaseLock(ref uint aLock)
        {
            aLock = 0;
        }
    }
}
