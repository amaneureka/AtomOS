using System;
using sys = System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;
using Kernel_alpha.x86.Intrinsic;

namespace Atomix.mscorlib
{
    public static class StringImpl
    {
        /*
            Length Offset   =>  [0x0C - 0x10)
            Data Offset     =>  [0x10 -  ∞)
        */
        [Plug("System_Void__System_String__ctor_System_Char___")]
        public static unsafe void ctor(byte* aFirstChar, char[] aChar)
        {
            ctor(aFirstChar, aChar, 0, aChar.Length);
        }

        [Plug("System_Void__System_String__ctor_System_Char____System_Int32__System_Int32_")]
        public static unsafe void ctor(byte* aFirstChar, char[] aChar, int Start, int Length)
        {
            byte* length = (byte*)(aFirstChar + 0xC);

            var _a = BitConverter.GetBytes(Length);
            length[0] = _a[0];
            length[1] = _a[1];
            length[2] = _a[2];
            length[3] = _a[3];

            char* chars = (char*)(aFirstChar + 0x10);
            for (int i = 0; i < Length; i++)
            {
                chars[i] = aChar[i + Start];
            }
            #warning TODO: Trim the last null chars
        }

        [Plug("System_Char_System_String_get_Chars_System_Int32_")]
        public static unsafe char Get_Chars(byte* aThis, int aIndex)
        {
            if (aIndex < 0 || aIndex >= Get_Length(aThis))
                return '\0';

            var xCharIdx = (char*)(aThis + 16);
            return xCharIdx[aIndex];
        }

        [Plug("System_Int32_System_String_get_Length__")]
        public unsafe static int Get_Length(byte* aThis)
        {
            var xCharIdx = (byte*)(aThis + 12);
            return (int)(xCharIdx[3] << 24 | xCharIdx[2] << 16 | xCharIdx[1] << 8 | xCharIdx[0]);
        }

        [Plug("System_String_System_String_Concat_System_String__System_String__System_String__System_String_")]
        public static string Concat(string s0, string s1, string s2, string s3)
        {
            return ConcatArray(new string[] { s0, s1, s2, s3 }, s0.Length + s1.Length + s2.Length + s3.Length);
        }

        [Plug("System_String_System_String_Concat_System_String__System_String__System_String_")]
        public static string Concat(string s0, string s1, string s2)
        {
            return ConcatArray(new string[] { s0, s1, s2 }, s0.Length + s1.Length + s2.Length);
        }

        [Plug("System_String_System_String_Concat_System_String__System_String_")]
        public static string Concat(string s0, string s1)
        {
            return ConcatArray(new string[] { s0, s1 }, s0.Length + s1.Length);
        }

        [Plug("System_String_System_String_Concat_System_String___")]
        public static string Concat(params string[] strs)
        {
            int len= 0;
            for (int i = 0; i < strs.Length; i++)
                len += strs[i].Length;

            return ConcatArray(strs, len);
        }

        private static string ConcatArray(string[] strs, int length)
        {
            char[] xResult = new char[length];
            int p = 0;
            for (int i = 0; i < strs.Length; i++)
            {
                var str = strs[i];
                for (int j = 0; j < str.Length; j++)
                {
                    xResult[p++] = str[j];
                }
            }

            return new String(xResult);
        }

        [Plug("System_String_System_String_Substring_System_Int32__System_Int32_")]
        public static string SubString(string aThis, int index, int length)
        {
            char[] xResult = new char[length];

            for (int i = 0; i < length; i++)
            {
                xResult[i] = aThis[index + i];
            }
            return new String(xResult);
        }

        [Plug("System_String_System_String_Substring_System_Int32_")]
        public static string SubString(string aThis, int index)
        {
            return SubString(aThis, index, aThis.Length - index + 1);
        }

        [Plug("System_String_System_String_ToLower__")]
        public static string ToLower(string aThis)
        {
            return ChangeCase(aThis, 65, 90, 32);
        }

        [Plug("System_String_System_String_ToUpper__")]
        public static string ToUpper(string aThis)
        {
            return ChangeCase(aThis, 97, 122, -32);
        }

        [Plug("System_String_System_String_PadLeft_System_Int32__System_Char_")]
        public static string PadLeft(string aThis, int TotalWidth, char paddingchar)
        {
            return Padding(aThis, TotalWidth, paddingchar, false);
        }

        [Plug("System_String_System_String_PadRight_System_Int32__System_Char_")]
        public static string PadRight(string aThis, int TotalWidth, char paddingchar)
        {
            return Padding(aThis, TotalWidth, paddingchar, true);
        }

        private static string Padding(string aThis, int TotalWidth, char PaddingChar, bool Direction)
        {
            var len = aThis.Length;

            if (len >= TotalWidth)
                return aThis;

            char[] xResult = new char[TotalWidth];
            
            if (Direction)
            {
                //Padding Right
                for (int i = 0; i < TotalWidth; i++)
                {
                    if (len <= i)
                        xResult[i] = PaddingChar;
                    else
                        xResult[i] = aThis[i];
                }
            }
            else
            {
                var xOffset = TotalWidth - len;
                //Padding Left
                for (int i = 0; i < TotalWidth; i++)
                {
                    if (i < xOffset)
                        xResult[i] = PaddingChar;
                    else
                        xResult[i] = aThis[i - xOffset];
                }
            }
            return new String(xResult);
        }

        private static string ChangeCase(string xStr, int lowerACII, int upperASCII, int value)
        {
            char[] xResult = new char[xStr.Length];
            for (int i = 0; i < xStr.Length; i++)
            {
                var xChar = xStr[i];
                if (xChar >= lowerACII && xChar <= upperASCII)
                {
                    xChar = (char)(xChar + value);
                }

                xResult[i] = xChar;
            }
            return new String(xResult);
        }

        [Plug("System_String___System_String_Split_System_Char___")]
        public static string[] Split(string str, char[] c)
        {
            int counter = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == c[0])
                {
                    counter++;
                }
            }
            string[] xResult = new string[counter + 1];
            char[] xTemp = new char[255];

            int mcounter = 0;
            int zcounter = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == c[0])
                {
                    char[] xTemp2 = new char[mcounter + 1];
                    for (int j = 0; j < mcounter; j++)
                    {
                        xTemp2[j] = xTemp[j];
                    }
                    mcounter = 0;
                    xResult[zcounter] = new string(xTemp2);
                    zcounter++;
                }
                else
                {
                    xTemp[mcounter] = str[i];
                    mcounter++;

                    if (i == str.Length - 1)
                    {
                        char[] xTemp2 = new char[mcounter + 1];
                        for (int j = 0; j < mcounter; j++)
                        {
                            xTemp2[j] = xTemp[j];
                        }
                        mcounter = 0;
                        xResult[zcounter] = new string(xTemp2);
                        zcounter++;
                    }
                }
            }
            return xResult;
        }

        [Plug("System_String_System_String_Trim__")]
        public static string Trim(string aThis)
        {
            int c = 0;
            for (int i = 0; i < aThis.Length; i++)
            {
                if (aThis[i] == ' ')
                    break;
                c++;
            }
            return aThis.Substring(0, c);
        }

        [Plug("System_String_System_String_Trim_System_Char___")]
        public static string Trim(string aThis, char[] aChar)
        {
            /* Done it in very hurry, haha...so it do limited work */
            int c = 0;
            for (int i = 0; i < aThis.Length; i++)
            {
                if (aThis[i] == aChar[0])
                    break;
                c++;
            }
            return aThis.Substring(0, c);
        }

        [Plug("System_Boolean_System_String_op_Equality_System_String__System_String_")]
        public static bool Equality(string str1, string str2)
        {
            var len = str1.Length;
            if (len != str2.Length)
                return false;

            for (int i = 0; i < len; i++)
            {
                if (str1[i] != str2[i])
                    return false;
            }
            return true;
        }
    }
}
