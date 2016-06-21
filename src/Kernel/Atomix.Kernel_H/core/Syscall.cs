/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          To Implement Syscall
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomix.Kernel_H.lib;
using Atomix.Kernel_H.arch.x86;

namespace Atomix.Kernel_H.core
{
    public static class Syscall
    {
        static IDictionary<uint, InterruptHandler> mCallHandlers;

        #region Enum
        /// <summary>
        /// https://sourceware.org/newlib/libc.html#Syscalls
        /// </summary>
        public enum Function : uint
        {
            exit            = 1,
            fork            = 2,
            read            = 3,
            write           = 4,
            open            = 5,
            close           = 6,
            wait4           = 7,
            creat           = 8,
            link            = 9,
            unlink          = 10,
            execv           = 11,
            chdir           = 12,
            mknod           = 14,
            chmod           = 15,
            chown           = 16,
            lseek           = 19,
            getpid          = 20,
            isatty          = 21,
            fstat           = 22,
            time            = 23,
            ARG             = 24,
            kill            = 37,
            stat            = 38,
            pipe            = 42,
            brk             = 45,
            execve          = 59,
            gettimeofday    = 78,
            truncate        = 129,
            ftruncate       = 130,
            argc            = 172,
            argnlen         = 173,
            argn            = 174,
            utime           = 201,
            wait            = 202,
        };
        #endregion

        public static void Setup()
        {
            mCallHandlers = new IDictionary<uint, InterruptHandler>(delegate (uint a)
            {
                return a;
            }, delegate (uint a, uint b)
            {
                return a==b;
            });

            /* Register Syscall Handler */
            IDT.RegisterInterrupt(Handler, 0x80);
        }

        public static void Register(Function aFunction, InterruptHandler aContext)
        {
            mCallHandlers.Add((uint)aFunction, aContext);
        }

        private static void Handler(ref IRQContext state)
        {
            try
            {
                if (mCallHandlers.ContainsKey(state.EAX))
                    mCallHandlers[state.EAX](ref state);
                else
                    throw new Exception("[Syscall]: Handler not found");
            }
            catch (Exception e)
            {
                state.ECX |= 0x2;// Mark it as fail
                Heap.Free(e);
            }
        }
    }
}
