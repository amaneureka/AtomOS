/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          File Contains various mscorlib plug belongs to Math class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomixilc;
using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;

namespace Atomix.Kernel_H.plugs
{
    internal static class Math
    {
        [Plug("System_Int32_System_Math_Max_System_Int32__System_Int32_")]
        internal static int Max(int a, int b)
        {
            return a >= b ? a : b;
        }

        [Plug("System_UInt32_System_Math_Max_System_UInt32__System_UInt32_")]
        internal static uint Max(uint a, uint b)
        {
            return a >= b ? a : b;
        }

        [Plug("System_Int32_System_Math_Min_System_Int32__System_Int32_")]
        internal static int Min(int a, int b)
        {
            return a >= b ? b : a;
        }

        [Plug("System_UInt32_System_Math_Min_System_UInt32__System_UInt32_")]
        internal static uint Min(uint a, uint b)
        {
            return a >= b ? b : a;
        }
    }
}
