/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is strictly prohibited
*                   Proprietary and confidential
* PURPOSE:
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomix.CompilerExt.Attributes;

namespace Atomix.Core.mscorlib
{
    public class Marshal
    {
        [Plug("System_IntPtr_System_Runtime_InteropServices_Marshal_AllocHGlobal_System_Int32_")]
        public static IntPtr AllocHGlobal()
        {
            return IntPtr.Zero;
        }

        [Plug("System_Void_System_Runtime_InteropServices_Marshal_FreeHGlobal_System_IntPtr_")]
        public static void FreeHGlobal(IntPtr hglobal)
        {

        }
    }
}
