/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          ASCII String Encoding
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomix.Kernel_H.Core;
using Atomix.Kernel_H.Arch.x86;

namespace Atomix.Kernel_H.Lib.encoding
{
    internal static class ASCII
    {
        internal static unsafe string GetString(byte* aData, int index, int length)
        {
            int newlen = length;
            for (int i = 0; i < length; i++)
            {
                if (aData[index + i] == 0x0)
                {
                    newlen = i;
                    break;
                }
            }

            char[] xResult = new char[newlen];
            for (int i = 0; i < newlen; i++)
            {
                xResult[i] = (char)aData[index + i];
            }

            var name = new String(xResult);
            Heap.Free(xResult);
            return name;
        }

        internal static unsafe string GetString(byte[] aData, int index, int length)
        {
            var add = (byte*)Native.GetContentAddress(aData);
            return GetString(add, index, length);
        }
    }
}
