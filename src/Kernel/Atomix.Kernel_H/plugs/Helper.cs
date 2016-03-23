/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* Copyright (c) 2015, Atomix Development, Inc - All Rights Reserved                                        *
*                                                                                                          *
* Unauthorized copying of this file, via any medium is strictly prohibited                                 *
* Proprietary and confidential                                                                             *
* Written by Aman Priyadarshi <aman.eureka@gmail.com>, February 2016                                       *
*                                                                                                          *
*   Namespace     ::  Atomix.Kernel_H.plugs                                                                *
*   File          ::  Helper.cs                                                                            *
*                                                                                                          *
*   Description                                                                                            *
*       File Contains various mscorlib plug that we don't want to compile                                  *
*                                                                                                          *
*   History                                                                                                *
*       14-02-2016      Aman Priyadarshi      Added Char Ctor Method                                       *
*       23-03-2016      Aman Priyadarshi      Added File Header                                            *
*                                                                                                          *
* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;

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
    }
}
