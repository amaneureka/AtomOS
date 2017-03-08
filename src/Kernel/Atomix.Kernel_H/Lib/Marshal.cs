/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Marshal extension functions
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomixilc.Lib;

using Atomix.Kernel_H.Core;

namespace Atomix.Kernel_H.Lib
{
    internal unsafe static class Marshal
    {
        internal static sbyte* C_String(string aStr)
        {
            int len = aStr.Length;
            sbyte* cstr = (sbyte*)Heap.kmalloc((uint)len + 1);

            int i = 0;
            while (len-- != 0)
                cstr[i] = (sbyte)aStr[i++];

            var str = new string(cstr);
            return cstr;
        }

        internal static void Copy(string aStr, sbyte* aCstr, int aLen)
        {
            int i = 0;
            while (aLen-- != 0)
                aCstr[i] = (sbyte)aStr[i++];
            aCstr[i] = 0;
        }

        internal static void Copy(char* aDes, string aSrc, int aLen)
        {
            Memory.FastCopy((uint)aDes, aSrc.GetDataOffset(), (uint)(aLen * sizeof(char)));
            aDes[aLen] = '\0';
        }

        internal static string[] Split(this string aStr, char aDelimiter)
        {
            var aArray = aStr.ToCharArray();

            int len = aStr.Length, count = 1;

            for (int i = 0; i < len; i++)
            {
                if (aArray[i] == aDelimiter)
                    count++;
            }

            int last = 0, index = 0;

            var strs = new string[count];
            for (int i = 0; i < len; i++)
            {
                if (aArray[i] == aDelimiter)
                {
                    strs[index++] = new string(aArray, last, i - last);
                    last = i + 1;
                }
            }

            if (index != count)
                strs[index] = new string(aArray, last, len - last);

            Heap.Free(aArray);
            return strs;
        }
    }
}
