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

using Atomixilc;
using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;

namespace Atomix.Kernel_H.plugs
{
    internal static class ExceptionImpl
    {
        [Plug("System_Void_System_Exception__cctor__")]
        internal static unsafe void ctor()
        {
            return;
        }

        [Plug("System_Void_System_Exception__ctor_System_String_")]
        internal static unsafe void cctor(byte* aAddress, uint Message)
        {
            *(uint*)(aAddress + 0xC) = Message;
        }

        [Plug("System_String_System_Exception_get_Message__")]
        internal static unsafe uint GetMessage(byte* aAddress)
        {
            return *(uint*)(aAddress + 0xC);
        }
    }
}
