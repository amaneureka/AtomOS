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
            int _l = aChar.Length;
                        
            byte* length = (byte*)(aFirstChar + 0xC);

            var _a = BitConverter.GetBytes(_l);
            length[0] = _a[0];
            length[1] = _a[1];
            length[2] = _a[2];
            length[3] = _a[3];
            
            char* chars = (char*)(aFirstChar + 0x10);
            for (int i = 0; i < _l; i++)
            {
                chars[i] = aChar[i];
            }
            #warning TODO: Trim the last null chars
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


    }
}
