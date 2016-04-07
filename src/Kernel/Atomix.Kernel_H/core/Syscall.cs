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
            /// <summary>
            /// Exit a program without cleaning up files.
            /// </summary>
            exit        =   0x0000001,
            /// <summary>
            /// Close a file.
            /// </summary>
            close       =   0x0000002,
            /// <summary>
            /// A pointer to a list of environment variables and their values.
            /// </summary>
            environ     =   0x0000003,
            /// <summary>
            /// Transfer control to a new process.
            /// </summary>
            execve      =   0x0000004,
            /// <summary>
            /// Create a new process.
            /// </summary>
            fork        =   0x0000005,
            /// <summary>
            /// Status of an open file. 
            /// </summary>
            fstat       =   0x0000006,
            /// <summary>
            /// Process-ID; this is sometimes used to generate strings unlikely to conflict with other processes.
            /// </summary>
            getpid      =   0x0000007,
            /// <summary>
            /// Query whether output stream is a terminal.
            /// </summary>
            isatty      =   0x0000008,
            /// <summary>
            /// Send a signal.
            /// </summary>
            kill        =   0x0000009,
            /// <summary>
            /// Establish a new name for an existing file.
            /// </summary>
            link        =   0x000000A,
            /// <summary>
            /// Set position in a file.
            /// </summary>
            lseek       =   0x000000B,
            /// <summary>
            /// Open a file.
            /// </summary>
            open        =   0x000000C,
            /// <summary>
            /// Read from a file
            /// </summary>
            read        =   0x000000D,
            /// <summary>
            /// Increase program data space. As malloc and related functions depend on this, 
            /// it is useful to have a working implementation
            /// </summary>
            sbrk        =   0x000000E,
            /// <summary>
            /// Status of a file (by name).
            /// </summary>
            stat        =   0x000000F,
            /// <summary>
            /// Timing information for current process
            /// </summary>
            times       =   0x0000010,
            /// <summary>
            /// Remove a file’s directory entry.
            /// </summary>
            unlink      =   0x0000011,
            /// <summary>
            /// Wait for a child process.
            /// </summary>
            wait        =   0x0000012,
            /// <summary>
            /// Write to a file. libc subroutines will use this system routine for output to all files, 
            /// including stdout—so if you need to generate any output, for example to a serial port for debugging, 
            /// you should make your minimal write capable of doing this. 
            /// </summary>
            write       =   0x0000013,
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
                state.ECX |= 0x2;//Mark it as fail
                Heap.Free(e);
            }
        }
    }
}
