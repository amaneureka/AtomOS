/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          File Contains various mscorlib plug belongs to String class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomixilc;
using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;

using Atomix.Kernel_H.Core;

namespace Atomix.Kernel_H.plugs
{
    internal static class String
    {
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
