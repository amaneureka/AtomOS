/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Libc native functions
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;
using System.Runtime.InteropServices;

using Atomixilc.Attributes;

namespace Atomixilc.Lib
{
    public static class Libc
    {
        const string LIBRARY = "libc.a";

        [NoException]
        [Plug("_init")]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Init();

        [NoException]
        [Plug("_fini")]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Finish();

        [NoException]
        [Plug("malloc")]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint malloc(int len);

        [NoException]
        [Plug("free")]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        public static extern void free(uint addr);
    }
}
