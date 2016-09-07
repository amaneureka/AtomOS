using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atomix.mscorlib
{
    public static class Number
    {
        public const string xDigits = "0123456789";
        public static string ToString8Bit(uint aNum, bool IsSigned)
        {
            if (aNum == 0)
                return "0";

            char[] xResult = new char[3];//0-255

            int c = 2;
            uint v = aNum;
            while (v > 0)
            {
                int r = (int)(v % 10);
                v /= 10;

                xResult[c] = xDigits[r];
                c--;
            }

            return new string(xResult);
        }

        public static string ToString16Bit(uint aNumber, bool IsSigned)
        {
            return string.Empty;
        }

        public static string ToString32Bit(uint aNumber, bool IsSigned)
        {
            uint xValue = aNumber;

            if (xValue == 0)
                return "0";

            char[] xResultChars = new char[11];
            int xCurrentPos = 10;
            
            while (xValue > 0)
            {
                byte xPos = (byte)(xValue % 10);
                xValue /= 10;
                xResultChars[xCurrentPos] = xDigits[xPos];
                xCurrentPos -= 1;
            }
            
            if (IsSigned)
            {
                xResultChars[xCurrentPos] = '-';
                xCurrentPos -= 1;
            }
            
            return new string(xResultChars, xCurrentPos + 1, 10 - xCurrentPos);
        }

        public static string ToString64Bit(ulong aNumber, bool IsSigned)
        {
            if (aNumber == 0)
                return "0";
            char[] xResultChars = new char[21];
            int xCurrentPos = 20;
            while (aNumber > 0)
            {
                byte xPos = (byte)(aNumber % 10);
                aNumber /= 10;
                xResultChars[xCurrentPos] = xDigits[xPos];
                xCurrentPos -= 1;
            }
            if (IsSigned)
            {
                xResultChars[xCurrentPos] = '-';
                xCurrentPos -= 1;
            }
            return new string(xResultChars, xCurrentPos + 1, 20 - xCurrentPos);
        }

        public static string ToStringHex(byte aByte)
        {
            const string xHex = "0123456789ABCDEF";
            char[] xResult = new char[2];
            xResult[0] = xHex[(int)((aByte >> 4) & 0xF)];
            xResult[1] = xHex[(int)(aByte & 0xF)];

            return new String(xResult);
        }
    }
}
