/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          File Contains various mscorlib exceptions
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomixilc;
using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;

namespace Atomix.Kernel_H.plugs
{
    internal static class NativeException
    {
        [Plug("System_Void_System_ThrowHelper_ThrowArgumentOutOfRangeException_System_ExceptionArgument__System_ExceptionResource_")]
        internal static void ThrowHelper(int Argument, int Resource)
        {
            return;
        }

        [Plug("System_Void_System_ThrowHelper_ThrowArgumentOutOfRangeException__")]
        internal static void ThrowArgumentOutOfRangeException()
        {
            return;
        }

        [Plug("System_Void_System_ThrowHelper_ThrowInvalidOperationException_System_ExceptionResource_")]
        internal static void ThrowInvalidOperationException(int ExceptionResource)
        {
            return;
        }
    }
}
