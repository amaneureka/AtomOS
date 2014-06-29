using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;

namespace Atomix.mscorlib
{
    public static class ExceptionImpl
    {
        /// <summary>
        /// This contains current error
        /// </summary>
        [Label("Exception")]
        public static string Exception;

        public static string Message;

        /// <summary>
        /// Well it is waste, just for compiler to build "exception" we set it string.empty
        /// </summary>
        [Plug("System_Void__System_Exception__cctor__")]
        public static void ctor()
        {
            Exception = string.Empty;
        }

        [Plug("System_Void__System_Exception__ctor_System_String_")]
        public static void ctor(string xm)
        {
            Message = xm;
        }

        /// <summary>
        /// Exception is pushed onto the stack (parameter xr)
        /// Just set its value as Message.
        /// </summary>
        /// <param name="xr"></param>
        [Plug("System_String_System_Exception_get_Message__")]
        public static void GetMessage(ref string xr)
        {
            xr = Message;
        }
    }
}
