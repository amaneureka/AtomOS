/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          File Contains various mscorlib plug belongs to Enum class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomix.CompilerExt.Attributes;

namespace Atomix.Kernel_H.plugs
{
    internal static unsafe class Enum
    {
        [Plug("System_Void__System_Enum__cctor__")]
        internal static void Cctor(byte* Address)
        {
            return;
        }
    }
}
