/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          File Contains various mscorlib plug belongs to Numerics class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomix.Kernel_H.Core;

using Atomixilc;
using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;

namespace Atomix.Kernel_H.plugs
{
    internal class Numerics
    {
        const string xDigits = "0123456789";

        [Plug("System_String_System_UInt32_ToString__")]
        internal static string ToString(ref uint aThis)
        {
            return ToString32Bit(aThis, false);
        }

        [Plug("System_String_System_Int32_ToString__")]
        internal static string ToString(ref int aThis)
        {
            if (aThis >= 0)
                return ToString32Bit((uint)aThis, false);
            else
                return ToString32Bit((uint)(-aThis), true);
        }

        [Plug("System_Int32_System_Int32_Parse_System_String_")]
        internal static int ParseInt32(string aStr)
        {
            int xStart = 0;

            bool IsNeg = false;
            if (aStr[0] == '-')
            {
                xStart++;
                IsNeg = true;
            }

            int len = aStr.Length;

            int value = 0;
            for (int i = xStart; i < len; i++)
            {
                var aChar = aStr[i];
                if (aChar >= '0' && aChar <= '9')
                    value = value * 10 + (int)(aChar - '0');
                else
                    break;
            }

            return (IsNeg ? -value : value);
        }

        [Plug("System_UInt32_System_UInt32_Parse_System_String_")]
        internal static uint ParseUInt32(string aStr)
        {
            uint value = 0;
            foreach (var aChar in aStr)
            {
                if (aChar >= '0' && aChar <= '9')
                    value = value * 10 + (uint)(aChar - '0');
                else
                    break;
            }
            return value;
        }

        internal static string ToString32Bit(uint aNum, bool Signed)
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
