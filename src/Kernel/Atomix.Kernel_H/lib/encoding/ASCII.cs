using System;

using Atomix.Kernel_H.core;
using Atomix.Kernel_H.arch.x86;

namespace Atomix.Kernel_H.lib.encoding
{
    public static class ASCII
    {
        public static unsafe string GetString(byte* aData, int index, int length)
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

        public static unsafe string GetString(byte[] aData, int index, int length)
        {
            var add = (byte*)(Native.GetAddress(aData) + 0x10);
            return GetString(add, index, length);
        }
    }
}
