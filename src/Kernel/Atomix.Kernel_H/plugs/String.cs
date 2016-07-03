/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          File Contains various mscorlib plug belongs to String class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomix.CompilerExt.Attributes;

using Atomix.Kernel_H.Core;

namespace Atomix.Kernel_H.plugs
{
    internal static class String
    {
        [Plug("System_Void__System_String__ctor_System_Char___")]
        internal static unsafe void ctor(byte* aFirstChar, char[] aChar)
        {
            ctor(aFirstChar, aChar, 0, aChar.Length);
        }

        [Plug("System_Void__System_String__ctor_System_Char__")]
        internal static unsafe void ctor(byte* aFirstChar, char* aChar)
        {
            int i = 0;
            char* chars = (char*)(aFirstChar + 0x10);
            while(*aChar != '\0')
            {
                *chars = *aChar;
                ++chars;
                ++aChar;
                ++i;
            }
            *((int*)(aFirstChar + 0xC)) = i;
        }

        [Plug("System_Void__System_String__ctor_System_Char____System_Int32__System_Int32_")]
        internal static unsafe void ctor(byte* aFirstChar, char[] aChar, int Start, int Length)
        {
            int i;
            char* chars = (char*)(aFirstChar + 0x10);
            for (i = 0; i < Length; i++)
                chars[i] = aChar[i + Start];
            
            *((int*)(aFirstChar + 0xC)) = i;
        }

        [Plug("System_Char_System_String_get_Chars_System_Int32_")]
        internal static unsafe char getChars(byte* aThis, int aIndex)
        {
            if (aIndex < 0 || aIndex >= getLength(aThis))
                return '\0';
            
            var xCharIdx = (char*)(aThis + 0x10);
            return xCharIdx[aIndex];
        }

        [Plug("System_Int32_System_String_get_Length__")]
        internal unsafe static int getLength(byte* aThis)
        {
            return *((int*)(aThis + 0xC));
        }

        [Label("getLength_System_Char__")]
        internal unsafe static int getLength(char* str)
        {
            int length = 0;
            while (*str != '\0')
            {
                ++str;
                ++length;
            }
            return length;
        }
        
        [Plug("System_String___System_String_Split_System_Char___")]
        internal static string[] Split(string str, params char[] delimiters)
        {
            bool[] IsDelimiter = new bool[255];// assuming that we will split string with only first 255 chars
            for (int index = 0; index < delimiters.Length; index++)
                IsDelimiter[delimiters[index]] = true;

            int Last = 0, parts = 0, MaximumLength = 0;
            for (int index = 0; index < str.Length; index++)
            {
                if (IsDelimiter[str[index]])
                {
                    parts++;
                    Last = index - Last;
                    if (Last > MaximumLength)
                        MaximumLength = Last;
                    Last = index + 1;
                }
            }

            if (MaximumLength == 0)
                MaximumLength = str.Length;

            var xResult = new string[parts + 1];
            var xTemp = new char[MaximumLength + 1];

            int xTempPointer = 0, xResPointer = 0;
            for (int index = 0; index < str.Length; index++)
            {
                if (IsDelimiter[str[index]])
                {
                    xResult[xResPointer++] = new string(xTemp, 0, xTempPointer);
                    xTempPointer = 0;
                }
                else
                    xTemp[xTempPointer++] = str[index];
            }

            xResult[xResPointer] = new string(xTemp, 0, xTempPointer);
            Heap.Free(xTemp);
            Heap.Free(IsDelimiter);
            Heap.Free(delimiters);
            return xResult;
        }

        [Plug("System_String_System_String_Concat_System_String___")]
        internal static string Concat(params string[] strs)
        {
            int TotalLength = 0;
            int strs_length = strs.Length;
            for (int i = 0; i < strs_length; i++)
                TotalLength += strs[i].Length;

            var char_result = new char[TotalLength];

            int p = 0;
            for (int i = 0; i < strs_length; i++)
            {
                var current = strs[i];
                TotalLength = current.Length;
                for (int j = 0; j < TotalLength; j++)
                    char_result[p++] = current[j];
            }

            var result = new string(char_result);
            Heap.Free(char_result);
            Heap.Free((object)strs);

            return result;
        }

        [Plug("System_Boolean_System_String_op_Equality_System_String__System_String_")]
        internal static bool opEquality(string str1, string str2)
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

        [Plug("System_Boolean_System_String_Equals_System_String__System_String_")]
        internal static bool Equals(string aStr1, string aStr2)
        {
            return opEquality(aStr1, aStr2);
        }

        [Plug("System_Boolean_System_String_op_Inequality_System_String__System_String_")]
        internal static bool opInEquality(string str1, string str2)
        {
            var len = str1.Length;
            if (len != str2.Length)
                return true;

            for (int i = 0; i < len; i++)
            {
                if (str1[i] != str2[i])
                    return true;
            }
            return false;
        }

        [Plug("System_Int32_System_String_IndexOf_System_Char__System_Int32__System_Int32_")]
        internal static int IndexOf(string str, char toFind, int start, int length)
        {
            int end = start + length;
            for (int index = start; index < end; index++)
                if (str[index] == toFind)
                    return index;
            return -1;
        }

        [Plug("System_Boolean_System_String_StartsWith_System_String_")]
        internal static bool StartsWith(string str, string toFind)
        {
            int length = str.Length;
            int length2 = toFind.Length;
            if (length < length2)
                return false;

            for (int index = 0; index < length2; index++)
            {
                if (str[index] != toFind[index])
                    return false;
            }
            return true;
        }

        [Plug("System_String_System_String_Concat_System_String__System_String__System_String__System_String_")]
        internal static string Concat(string s0, string s1, string s2, string s3)
        {
            return Concat(new string[] { s0, s1, s2, s3 });
        }

        [Plug("System_String_System_String_Concat_System_String__System_String__System_String_")]
        internal static string Concat(string s0, string s1, string s2)
        {
            return Concat(new string[] { s0, s1, s2 });
        }

        [Plug("System_String_System_String_Concat_System_String__System_String_")]
        internal static string Concat(string s0, string s1)
        {
            return Concat(new string[] { s0, s1 });
        }
    }
}
