/* Copyright (C) Atomix Development, Inc - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Aman Priyadarshi <aman.eureka@gmail.com>, December 2014
 * 
 * String.cs
 *      .Net string implementation plug
 *      
 *      History:
 *          19-12-14    File Created    Aman Priyadarshi
 */

using System;
using System.Collections.Generic;

using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;
using Atomix.Kernel_H.core;

namespace Atomix.Kernel_H.plugs
{
    public static class String
    {
        [Plug("System_Void__System_String__ctor_System_Char___")]
        public static unsafe void ctor(byte* aFirstChar, char[] aChar)
        {
            ctor(aFirstChar, aChar, 0, aChar.Length);
        }

        [Plug("System_Void__System_String__ctor_System_Char____System_Int32__System_Int32_")]
        public static unsafe void ctor(byte* aFirstChar, char[] aChar, int Start, int Length)
        {
            int i;
            char* chars = (char*)(aFirstChar + 0x10);
            for (i = 0; i < Length; i++)
            {
                var temp = aChar[i + Start];
                chars[i] = temp;
            }

            byte* length = (byte*)(aFirstChar + 0xC);

            length[0] = (byte)(i & 0xFF);
            length[1] = (byte)(i & 0xFF00);
            length[2] = (byte)(i & 0xFF0000);
            length[3] = (byte)(i & 0xFF000000);
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
                    char[] xTemp2 = new char[mcounter];
                    for (int j = 0; j < mcounter; j++)
                    {
                        xTemp2[j] = xTemp[j];
                    }
                    mcounter = 0;
                    xResult[zcounter++] = new string(xTemp2);
                }
                else
                {
                    xTemp[mcounter++] = str[i];
                    if (i == str.Length - 1)
                    {
                        char[] xTemp2 = new char[mcounter];
                        for (int j = 0; j < mcounter; j++)
                        {
                            xTemp2[j] = xTemp[j];
                        }
                        xResult[zcounter] = new string(xTemp2);
                    }
                }
            }
            return xResult;
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
