/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Apply a mutual exclusive lock to thread also implements `lock`keyword
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomix.Kernel_H.lib;
using Atomix.Kernel_H.arch.x86;

using Atomix.CompilerExt.Attributes;

namespace Atomix.Kernel_H.core
{
    internal static class Monitor
    {
        // Holds Object which are locked and thread id which has acquired the lock
        static IDictionary<object, int> mLocks;

        internal static void Setup()
        {
            mLocks = new IDictionary<object, int>(delegate (object aObj)
            {
                // If two objects shares the same address
                return Native.GetAddress(aObj);
            }, object.Equals);
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
