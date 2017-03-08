/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;

namespace Atomix.Core.mscorlib
{
    public class Exception
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

        [Label(Helper.lblSetException)]
        public static void SetException(Exception aException)
        {
            return;
        }

        [Label(Helper.lblGetException)]
        public static Exception GetException()
        {
            return null;
        }
    }
}
