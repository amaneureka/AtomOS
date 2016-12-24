/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          To Implement Syscall
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomix.Kernel_H.Arch.x86;

namespace Atomix.Kernel_H.Core
{
    internal static class Syscall
    {
        static InterruptHandler[] functions;

        internal static void Setup()
        {
            functions = new InterruptHandler[256];

            IDT.RegisterInterrupt(Handler, 0x7F);

            functions[(int)Function.SYS_brk] = sys_brk;
        }

        private static void Handler(ref IRQContext context)
        {
            if (context.EAX >= functions.Length)
            {
                Debug.Write("Invalid call\n");
                return;
            }

            var Handler = functions[context.EAX];

            if (Handler == null)
            {
                Debug.Write("syscall handler not found : %d\n", context.EAX);
                return;
            }

            Handler(ref context);
        }

        private static void sys_brk(ref IRQContext context)
        {
            if ((Function)context.EAX != Function.SYS_brk)
                return;

            context.EAX = Heap.kmalloc(context.EBX);
        }

        /// <summary>
        /// Defined in Syscalls.h
        /// </summary>
        internal enum Function : uint
        {
            SYS_exit = 1,
            SYS_fork = 2,
            SYS_read = 3,
            SYS_write = 4,
            SYS_open = 5,
            SYS_close = 6,
            SYS_wait4 = 7,
            SYS_creat = 8,
            SYS_link = 9,
            SYS_unlink = 10,
            SYS_execv = 11,
            SYS_chdir = 12,
            SYS_mknod = 14,
            SYS_chmod = 15,
            SYS_chown = 16,
            SYS_lseek = 19,
            SYS_getpid = 20,
            SYS_isatty = 21,
            SYS_fstat = 22,
            SYS_time = 23,
            SYS_ARG = 24,
            SYS_kill = 37,
            SYS_stat = 38,
            SYS_pipe = 42,
            SYS_brk = 45,
            SYS_execve = 59,
            SYS_gettimeofday = 78,
            SYS_truncate = 129,
            SYS_ftruncate = 130,
            SYS_argc = 172,
            SYS_argnlen = 173,
            SYS_argn = 174,
            SYS_utime = 201,
            SYS_wait = 202
        };
    }
}
