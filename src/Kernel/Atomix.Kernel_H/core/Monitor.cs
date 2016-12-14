/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Apply a mutual exclusive lock to thread also implements `lock`keyword
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomix.Kernel_H.Lib;
using Atomix.Kernel_H.Arch.x86;

using Atomixilc.Lib;
using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;

namespace Atomix.Kernel_H.Core
{
    internal static class Monitor
    {
        // Holds Object which are locked and thread id which has acquired the lock
        static IDictionary<object, int> mLocks;

        internal static void Setup()
        {
            mLocks = new IDictionary<object, int>(Native.GetAddress, Equals);
        }

        internal static void AcquireLock(object aObj)
        {
            bool Lock = false;
            AcquireLock(aObj, ref Lock);
        }

        [Label("AcquireLock")]
        // https://msdn.microsoft.com/en-us/library/dd289498(v=vs.110).aspx
        internal static void AcquireLock(object aObj, ref bool aLockTaken)
        {
            aLockTaken = false;

            int ThreadID = Scheduler.RunningThreadID;
            int ID = mLocks.GetValue(aObj, -1);

            if (ThreadID == ID)// If thread has already acquired lock
                return;

            do
            {
                mLocks.SafeAdd(aObj, ThreadID);
                ID = mLocks[aObj];
#warning Add thread sleep
            }
            while (ID != ThreadID);// Make sure we have owned the lock

            aLockTaken = true;
        }

        [Label("ReleaseLock")]
        // https://msdn.microsoft.com/en-in/library/system.threading.monitor.exit(v=vs.110).aspx
        internal static void ReleaseLock(object aObj)
        {
            int ThreadID = Scheduler.RunningThreadID;
            int ID = mLocks.GetValue(aObj, -1);

            if (ThreadID == ID)// If thread has already acquired lock
            {
                mLocks.RemoveKey(aObj);
                return;
            }
            throw new Exception("[Release Lock]: This thread has not owned the lock");
        }
    }
}
