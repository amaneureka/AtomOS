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
            char tmp;
            int i;
            char* chars = (char*)(aFirstChar + 0x10);
            for (i = 0; i < Length; i++)
            {
                tmp = aChar[i + Start];
                if (tmp == '\0')
                    break;
                chars[i] = tmp;
            }

            uint* length = (uint*)(aFirstChar + 0xC);

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
    }
}
