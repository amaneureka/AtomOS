/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* Copyright (c) 2015, Atomix Development, Inc - All Rights Reserved                                        *
*                                                                                                          *
* Unauthorized copying of this file, via any medium is strictly prohibited                                 *
* Proprietary and confidential                                                                             *
* Written by Aman Priyadarshi <aman.eureka@gmail.com>, Decmber 2014                                        *
*                                                                                                          *
*   Namespace     ::  Atomix.Kernel_H.plugs                                                                *
*   File          ::  Exception.cs                                                                         *
*                                                                                                          *
*   Description                                                                                            *
*       File Contains various mscorlib plug belongs to Exception class                                     *
*                                                                                                          *
*   History                                                                                                *
*       19-12-2014      Aman Priyadarshi      Added Basic Exception Handler                                *
*       06-02-2016      Aman Priyadarshi      Better Exception Handler (Multiprocess)                      *
*       23-03-2016      Aman Priyadarshi      Added File Header                                            *
*                                                                                                          *
* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;

using Atomix.Kernel_H.core;

using Atomix.CompilerExt.Attributes;

namespace Atomix.Kernel_H.plugs
{
    public static class ExceptionImpl
    {
        [Plug("System_Void__System_Exception__cctor__")]
        public static unsafe void ctor()
        {
            return;
        }

        [Plug("System_Void__System_Exception__ctor_System_String_")]
        public static unsafe void cctor(byte* aAddress, uint Message)
        {
            *(uint*)(aAddress + 0xC) = Message;
        }
        
        [Plug("System_String_System_Exception_get_Message__")]
        public static unsafe uint GetMessage(byte* aAddress)
        {
            return *(uint*)(aAddress + 0xC);
        }

        [Label("SetException")]
        public static void SetException(Exception aException)
        {
            var Thread = Scheduler.CurrentThread;
            if (Thread != null)
                Thread.Exception = aException;
            Debug.Write("[SetException]: %s\n", aException.Message);
        }

        [Label("GetException")]
        public static Exception GetException()
        {
            var Thread = Scheduler.CurrentThread;
            if (Thread != null)
                return Thread.Exception;
            return null;
        }
    }
}
