/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is strictly prohibited
*                   Proprietary and confidential
* PURPOSE:          File Contains various mscorlib plug
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomix.CompilerExt.Attributes;

namespace Atomix.Kernel_H.plugs
{
    public static class Helper
    {
        /// <summary>
        /// Dummy plug don't let the compiler to compile method with given signature
        /// </summary>
        [Dummy, Plug("System_Void__System_Char__cctor__")]
        public static void Char_ctor()
        {

        }

        [Plug("System_Boolean_System_Object_Equals_System_Object_")]
        public static bool Object_Equals(uint aObjA, uint aObjB)//Treat it as address
        {
            return (aObjA == aObjB);
        }
    }
}
