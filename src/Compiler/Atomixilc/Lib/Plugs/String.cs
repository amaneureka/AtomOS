/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          System.String Plugs
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;

namespace Atomixilc.Lib.Plugs
{
    internal unsafe static class StringImpl
    {
        [NoException]
        [Plug("System_Void_System_String__ctor_System_Char___")]
        internal static void String(byte* aStr, char[] aArray)
        {
            String(aStr, aArray, 0, aArray.Length);
        }

        [NoException]
        [Plug("System_Void_System_String__ctor_System_Char__")]
        internal static void String(byte* aStr, char* aArray)
        {
            var aChar = (char*)(aStr + 0x10);

            int len = 0;
            while (*aArray != '\0')
            {
                *(aChar++) = *(aArray++);
                len++;
            }

            *((int*)(aStr + 0xC)) = len;
        }

        [NoException]
        [Plug("System_Void_System_String__ctor_System_SByte__")]
        internal static void String(byte* aStr, sbyte* aArray)
        {
            var aChar = (char*)(aStr + 0x10);

            int len = 0;
            while (*aArray != 0)
            {
                *(aChar++) = (char)*(aArray++);
                len++;
            }

            *((int*)(aStr + 0xC)) = len;
        }

        [NoException]
        [Plug("System_Void_System_String__ctor_System_SByte___System_Int32__System_Int32_")]
        internal static void String(byte* aStr, sbyte* aArray, int start, int length)
        {
            var aChar = (char*)(aStr + 0x10);
            aArray += start;

            int len = 0;
            while (*aArray != 0)
            {
                *(aChar++) = (char)*(aArray++);
                len++;

                if (len == length) break;
            }

            *((int*)(aStr + 0xC)) = len;
        }

        [NoException]
        [Assembly(true)]
        [Plug("System_Void_System_String__ctor_System_Char____System_Int32__System_Int32_")]
        internal static void String(byte* aStr, char[] aArray, int aStart, int aLength)
        {
            new Mov { DestinationReg = Register.ECX, SourceReg = Register.EBP, SourceDisplacement = 8, SourceIndirect = true };
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.EBP, SourceDisplacement = 12, SourceIndirect = true };
            new Add { DestinationReg = Register.EAX, SourceReg = Register.EBP, SourceDisplacement = 12, SourceIndirect = true };
            new Add { DestinationReg = Register.EAX, SourceRef = "0x10" };

            new Mov { DestinationReg = Register.ESI, SourceReg = Register.EBP, SourceDisplacement = 16, SourceIndirect = true };
            new Add { DestinationReg = Register.ESI, SourceReg = Register.EAX };

            new Mov { DestinationReg = Register.EDI, SourceReg = Register.EBP, SourceDisplacement = 20, SourceIndirect = true };
            new Mov { DestinationReg = Register.EDI, DestinationDisplacement = 0xC, DestinationIndirect = true, SourceReg = Register.ECX };
            new Add { DestinationReg = Register.EDI, SourceRef = "0x10" };
            new Literal("rep movsw");
        }
        
        [NoException]
        [Plug("System_Char_System_String_get_Chars_System_Int32_")]
        internal static char GetChar(byte* aStr, int index)
        {
            return *((char*)(aStr + 0x10 + (index<<1)));
        }

        [NoException]
        [Plug("System_Int32_System_String_get_Length__")]
        internal static int GetLength(byte* aStr)
        {
            return *((int*)(aStr + 0xC));
        }

        [NoException]
        [Label("GetLength.System.Char*")]
        internal static int GetLength(char* aArray)
        {
            int len = 0;
            while (*(aArray++) != '\0')
                len++;
            return len;
        }

        [NoException]
        [Label("GetLength.System.SByte*")]
        internal static int GetLength(sbyte* aArray)
        {
            int len = 0;
            while (*(aArray++) != 0)
                len++;
            return len;
        }

        [NoException]
        [Plug("System_Char___System_String_ToCharArray__")]
        internal static char[] ToCharArray(string aStr)
        {
            int len = aStr.Length;
            var aArray = new char[len];
            Memory.FastCopy(aArray.GetDataOffset(), aStr.GetDataOffset(), (uint)(len << 1));

            return aArray;
        }
    }
}
