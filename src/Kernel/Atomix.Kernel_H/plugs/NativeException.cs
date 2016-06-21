/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          File Contains various mscorlib exceptions
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomix.CompilerExt.Attributes;

namespace Atomix.Kernel_H.plugs
{
    public static class NativeException
    {
        [Plug("System_Void_System_ThrowHelper_ThrowArgumentOutOfRangeException_System_ExceptionArgument__System_ExceptionResource_")]
        public static void ThrowHelper(int Argument, int Resource)
        {
            return;
        }

        [Plug("System_Void_System_ThrowHelper_ThrowArgumentOutOfRangeException__")]
        public static void ThrowArgumentOutOfRangeException()
        {
            return;
        }

        [Plug("System_Void_System_ThrowHelper_ThrowInvalidOperationException_System_ExceptionResource_")]
        public static void ThrowInvalidOperationException(int ExceptionResource)
        {
            return;
        }
    }
}
