using System;

using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;

namespace Atomix.Kernel_H.plugs
{
    public static class Math
    {
        [Plug("System_Int32_System_Math_Max_System_Int32__System_Int32_")]
        public static int Max(int a, int b)
        {
            return a >= b ? a : b;
        }

        [Plug("System_Int32_System_Math_Min_System_Int32__System_Int32_")]
        public static int Min(int a, int b)
        {
            return a >= b ? b : a;
        }
    }
}
