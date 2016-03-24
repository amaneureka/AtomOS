/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* Copyright (c) 2015, Atomix Development, Inc - All Rights Reserved                                        *
*                                                                                                          *
* Unauthorized copying of this file, via any medium is strictly prohibited                                 *
* Proprietary and confidential                                                                             *
* Written by Aman Priyadarshi <aman.eureka@gmail.com>, March 2016                                          *
*                                                                                                          *
*   Namespace     ::  Atomix.Kernel_H.core                                                                 *
*   File          ::  Monitor.cs                                                                           *
*                                                                                                          *
*   Description                                                                                            *
*       Apply a mutual exclusive lock to thread also implements `lock`keyword                              *
*                                                                                                          *
*   History                                                                                                *
*       24-03-2016      Aman Priyadarshi      Added Required Functions                                     *
*                                                                                                          *
* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;

using Atomix.Kernel_H.lib;
using Atomix.Kernel_H.arch.x86;

using Atomix.CompilerExt.Attributes;

namespace Atomix.Kernel_H.core
{
    public static class Monitor
    {
        //Holds Object which are locked and thread id which has acquired the lock
        static IDictionary<object, int> AcquiredLock;

        public static void Setup()
        {
            AcquiredLock = new IDictionary<object, int>(delegate (object aObj)
            {
                //If two objects shares the same address
                return Native.GetAddress(aObj);
            }, object.Equals);
        }
                
        [Label("AcquireLock")]
        //https://msdn.microsoft.com/en-us/library/dd289498(v=vs.110).aspx
        public static void AcquireLock(object aObj, ref bool aLockTaken)
        {
            aLockTaken = false;

            int ID = -1;
            int ThreadID = Scheduler.RunningThreadID;

            ID = AcquiredLock.GetValue(aObj, -1);
            if (ThreadID == ID)//If thread has already acquired lock
                return;

            do
            {
                AcquiredLock.SafeAdd(aObj, ThreadID);
                ID = AcquiredLock[aObj];

#warning Add thread sleep
            }
            while (ID != ThreadID);//Make sure we have owned the lock
            
            aLockTaken = true;
        }

        [Label("ReleaseLock")]
        //https://msdn.microsoft.com/en-in/library/system.threading.monitor.exit(v=vs.110).aspx
        public static void ReleaseLock(object aObj)
        {
            int ID = -1;
            int ThreadID = Scheduler.RunningThreadID;

            ID = AcquiredLock.GetValue(aObj, -1);
            if (ThreadID == ID)//If thread has already acquired lock
                AcquiredLock.RemoveKey(aObj);

            throw new Exception("[Release Lock]: This thread has not owned the lock");
        }
    }
}
