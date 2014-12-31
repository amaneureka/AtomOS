/* Copyright (C) Atomix Development, Inc - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Aman Priyadarshi <aman.eureka@gmail.com>, December 2014
 * 
 * Exception.cs
 *      .Net Exception class implementation plug
 *      
 *      History:
 *          19-12-14    File Created    Aman Priyadarshi
 */

using System;
using System.Collections.Generic;

using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;

namespace Atomix.Kernel_H.plugs
{
    public static class Exception
    {
        /// <summary>
        /// Compiler Inbuilt Exception pointer
        /// </summary>
        [Label("Exception")]
        public static string pointer;

        [Plug("System_Void__System_Exception__cctor__")]
        public static unsafe void ctor()
        {
            return;
        }

        [Plug("System_Void__System_Exception__ctor_System_String_")]
        public static unsafe void ctor2(byte* aAddress, uint aMessage)
        {
            *(uint*)(aAddress + 0xC) = aMessage;
        }

        [Plug("System_String_System_Exception_get_Message__")]
        public static unsafe uint GetMessage(byte* aAddress)
        {
            return *(uint*)(aAddress + 0xC);
        }
    }
}
