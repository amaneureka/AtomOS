/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* Copyright (c) 2015, Atomix Development, Inc - All Rights Reserved                                        *
*                                                                                                          *
* Unauthorized copying of this file, via any medium is strictly prohibited                                 *
* Proprietary and confidential                                                                             *
* Written by Aman Priyadarshi <aman.eureka@gmail.com>, January 2016                                        *
*                                                                                                          *
*   Namespace     ::  Atomix.Kernel_H.plugs                                                                *
*   File          ::  Numerics.cs                                                                          *
*                                                                                                          *
*   Description                                                                                            *
*       File Contains various mscorlib plug belongs to Numerics class                                      *
*                                                                                                          *
*   History                                                                                                *
*       04-01-2016      Aman Priyadarshi      Added ToString Function                                      *
*       06-02-2016      Aman Priyadarshi      Memory Mangement Fixes                                       *
*       23-03-2016      Aman Priyadarshi      Added Integer Parse Function                                 *
*       23-03-2016      Aman Priyadarshi      Added File Header                                            *
*                                                                                                          *
* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;

using Atomix.Kernel_H.core;

using Atomix.CompilerExt.Attributes;

namespace Atomix.Kernel_H.plugs
{
    public class Numerics
    {
        const string xDigits = "0123456789";

        [Plug("System_String_System_UInt32_ToString__")]
        public static string ToString(ref uint aThis)
        {
            return ToString32Bit(aThis, false);
        }

        [Plug("System_String_System_Int32_ToString__")]
        public static string ToString(ref int aThis)
        {
            if (aThis >= 0)
                return ToString32Bit((uint)aThis, false);
            else
                return ToString32Bit((uint)(-aThis), true);
        }

        [Plug("System_UInt32_System_UInt32_Parse_System_String_")]
        public static int ParseInt32(string aStr)
        {
            int aValue = 0;
#warning Add Invalid format exception
            foreach (var aChar in aStr)
            {
                if (aChar == '-')
                    continue;
                aValue = (aValue * 10) + (aChar - '0');
            }
            return (aStr[0] == '-' ? -aValue : aValue);
        }

        [Plug("System_UInt32_System_UInt32_Parse_System_String_")]
        public static uint ParseUInt32(string aStr)
        {
            uint aValue = 0;
#warning Add Invalid format exception
            foreach(var aChar in aStr)
                aValue = (aValue * 10) + (uint)(aChar - '0');
            return aValue;
        }

        public static string ToString32Bit(uint aNum, bool Signed)
        {
            var xResult = new char[11];
            int xPos = 11;
            if (aNum == 0)
                xResult[--xPos] = xDigits[0];

            while(aNum > 0)
            {
                xResult[--xPos] = xDigits[(int)(aNum % 10)];
                aNum /= 10;
            }

            if (Signed)
                xResult[--xPos] = '-';

            var xStr = new string(xResult, xPos, 11 - xPos);
            Heap.Free(xResult);
            return xStr;
        }
    }
}
