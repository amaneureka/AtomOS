/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* Copyright (c) 2015, Atomix Development, Inc - All Rights Reserved                                        *
*                                                                                                          *
* Unauthorized copying of this file, via any medium is strictly prohibited                                 *
* Proprietary and confidential                                                                             *
* Written by Aman Priyadarshi <aman.eureka@gmail.com>, December 2015                                       *
*                                                                                                          *
*   Namespace     ::  Atomix.Kernel_H.plugs                                                                *
*   File          ::  Math.cs                                                                              *
*                                                                                                          *
*   Description                                                                                            *
*       File Contains various mscorlib plug belongs to Math class                                          *
*                                                                                                          *
*   History                                                                                                *
*       16-12-2015      Aman Priyadarshi      Added Max, Min Function                                      *
*       23-03-2016      Aman Priyadarshi      Added File Header                                            *
*                                                                                                          *
* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;

using Atomix.CompilerExt.Attributes;

namespace Atomix.Kernel_H.plugs
{
    public static class Math
    {
        [Plug("System_Int32_System_Math_Max_System_Int32__System_Int32_")]
        public static int Max(int a, int b)
        {
            return a >= b ? a : b;
        }

        [Plug("System_Int32_System_Math_Min_System_Int32__System_Int32_")]
        public static int Min(int a, int b)
        {
            return a >= b ? b : a;
        }
    }
}
