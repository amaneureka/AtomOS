/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* Copyright (c) 2015, Atomix Development, Inc - All Rights Reserved                                        *
*                                                                                                          *
* Unauthorized copying of this file, via any medium is strictly prohibited                                 *
* Proprietary and confidential                                                                             *
* Written by Aman Priyadarshi <aman.eureka@gmail.com>, March 2015                                          *
*                                                                                                          *
*   Namespace     ::  Atomix.Kernel_H.plugs                                                                *
*   File          ::  Enum.cs                                                                              *
*                                                                                                          *
*   Description                                                                                            *
*       File Contains various mscorlib plug belongs to Enum class                                          *
*                                                                                                          *
*   History                                                                                                *
*       26-03-2015      Aman Priyadarshi      Added Ctor Method                                            *
*       23-03-2016      Aman Priyadarshi      Added File Header                                            *
*                                                                                                          *
* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;

using Atomix.CompilerExt.Attributes;

namespace Atomix.Kernel_H.plugs
{
    public static unsafe class Enum
    {
        [Plug("System_Void__System_Enum__cctor__")]
        public static void Cctor(byte* Address)
        {
            return;
        }
    }
}
