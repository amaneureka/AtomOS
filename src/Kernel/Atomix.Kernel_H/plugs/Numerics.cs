using System;

using Atomix.Kernel_H.core;

using Atomix.CompilerExt;
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
