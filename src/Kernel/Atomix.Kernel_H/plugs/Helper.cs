/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          File Contains various mscorlib plug
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomixilc;
using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;

namespace Atomix.Kernel_H.plugs
{
    internal static class PlugHelper
    {
        /// <summary>
        /// Dummy plug don't let the compiler to compile method with given signature
        /// </summary>
        [Plug("System_Void_System_Char__cctor__")]
        internal static void Char_ctor()
        {

        }

        [Plug("System_Boolean_System_Object_Equals_System_Object_")]
        internal static bool Object_Equals(uint aObjA, uint aObjB)// Treat it as address
        {
            return (aObjA == aObjB);
        }
    }
}
