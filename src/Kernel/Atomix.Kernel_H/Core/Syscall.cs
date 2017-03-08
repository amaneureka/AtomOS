/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          To Implement Syscall
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomix.Kernel_H.IO;
using Atomix.Kernel_H.Arch.x86;
using Atomix.Kernel_H.IO.FileSystem;

namespace Atomix.Kernel_H.Core
{
    internal static class Syscall
    {
        internal static void Setup()
        {
            IDT.RegisterInterrupt(Handler, 0x7F);
        }

        private static unsafe void Handler(ref IRQContext context)
        {
            switch((Function)context.EAX)
            {
                case Function.SYS_brk:
                    context.EAX = (int)sys_brk(context.EBX);
                    break;
                case Function.SYS_read:
                    context.EAX = sys_read(context.EBX, (byte*)context.ECX, context.EDX);
                    break;
                case Function.SYS_open:
                    context.EAX = sys_open((sbyte*)context.EBX, context.ECX, context.EDX);
                    break;
                case Function.SYS_seek:
                    context.EAX = sys_seek(context.EBX, context.ECX, context.EDX);
                    break;
                case Function.SYS_close:
                    context.EAX = sys_close(context.EBX);
                    break;
                case Function.SYS_write:
                    context.EAX = sys_write(context.EBX, (byte*)context.ECX, context.EDX);
                    break;
                default:
                    Debug.Write("Unhandled syscall %d\n", context.EAX);
                    break;
            }
        }

        private static unsafe uint sys_brk(int len)
        {
            var Process = Scheduler.RunningProcess;

            uint directory = Process.PageDirectory;
            uint current = Process.HeapCurrent;
            uint end = Process.HeapEndAddress;

            // Assert EAX == sys_brk

            uint addr = current;
            current += (uint)len;

            if (current > 0xA5000000)
            {
                Debug.Write("sys_brk failed\n");
                return 0;
            }

            // Assert end should be page aligned

            while (current > end)
            {
                Paging.AllocateFrame(Paging.GetPage((uint*)directory, end, true), 0, true);
                Paging.InvalidatePageAt(end);
                end += Paging.PageSize;
            }

            Process.HeapCurrent = current;
            Process.HeapEndAddress = end;

            return addr;
        }

        private static unsafe int sys_write(int fb, byte* buffer, int count)
        {
            for (int i = 0; i < count; i++)
                Debug.Write(*(buffer + i));
            return count;
        }

        private static unsafe int sys_read(int fd, byte* buffer, int count)
        {
            if (fd < 0) return -1;

            var Process = Scheduler.RunningProcess;
            var files = Process.Files;

            if (fd >= files.Count) return -1;

            var stream = Process.Files[fd];
            return stream.Read(buffer, count);
        }

        private static int sys_seek(int fd, int offset, int origin)
        {
            if (fd < 0) return -1;

            var Process = Scheduler.RunningProcess;
            var files = Process.Files;

            if (fd >= files.Count) return -1;
            if (origin > 2) return -1;

            var stream = Process.Files[fd];
            return stream.Seek(offset, (SEEK)origin);
        }

        private static int sys_close(int fd)
        {
            if (fd < 0) return -1;

            var Process = Scheduler.RunningProcess;
            var files = Process.Files;

            if (fd >= files.Count) return -1;

            var stream = Process.Files[fd];
            Process.Files[fd] = null;

            stream.Close();
            Debug.Write("close() : %d\n", fd);

            return 0;
        }

        private static unsafe int sys_open(sbyte* file, int flags, int mode)
        {
            var filename = new string(file);
            var stream = VirtualFileSystem.GetFile(filename);
            Debug.Write("fopen: %s\n", filename);
            Heap.Free(filename);

            if (stream == null)
                return -1;

            var Process = Scheduler.RunningProcess;
            var files = Process.Files;
            int count = files.Count;

            int fd = -1;
            for (int index = 0; index < count; index++)
            {
                if (files[index] == null)
                {
                    files[index] = stream;
                    fd = index;
                }
            }

            if (fd == -1)
            {
                files.Add(stream);
                fd = count;
            }

            return fd;
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
            SYS_seek = 19,
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
