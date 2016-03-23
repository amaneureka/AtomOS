/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* Copyright (c) 2015, Atomix Development, Inc - All Rights Reserved                                        *
*                                                                                                          *
* Unauthorized copying of this file, via any medium is strictly prohibited                                 *
* Proprietary and confidential                                                                             *
* Written by Aman Priyadarshi <aman.eureka@gmail.com>, March 2015                                          *
*                                                                                                          *
*   Namespace     ::  Atomix.Kernel_H.plugs                                                                *
*   File          ::  NativeException.cs                                                                   *
*                                                                                                          *
*   Description                                                                                            *
*       File Contains various mscorlib exceptions                                                          *
*                                                                                                          *
*   History                                                                                                *
*       26-03-2015      Aman Priyadarshi      Added Exceptions                                             *
*       23-03-2016      Aman Priyadarshi      Added File Header                                            *
*                                                                                                          *
* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;

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
