/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          File Contains various mscorlib plug belongs to Exception class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomix.Kernel_H.Core;

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
        
        [Label(CompilerExt.Helper.lblSetException)]
        public static void SetException(Exception aException)
        {
            var Thread = Scheduler.RunningThread;
            if (Thread != null)
                Thread.Exception = aException;
            Debug.Write("[SetException]: %s\n", aException.Message);
        }

        [Label(CompilerExt.Helper.lblGetException)]
        public static Exception GetException()
        {
            var Thread = Scheduler.RunningThread;
            if (Thread != null)
                return Thread.Exception;
            return null;
        }
    }
}
