/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          File Contains various mscorlib plug belongs to Math class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomix.CompilerExt.Attributes;

namespace Atomix.Kernel_H.plugs
{
    internal static class Math
    {
        [Plug("System_Int32_System_Math_Max_System_Int32__System_Int32_")]
        internal static int Max(int a, int b)
        {
            return a >= b ? a : b;
        }

        [Plug("System_Int32_System_Math_Min_System_Int32__System_Int32_")]
        internal static int Min(int a, int b)
        {
            return a >= b ? b : a;
        }
    }
}
