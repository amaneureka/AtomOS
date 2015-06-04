using System;

using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;
using Atomix.Kernel_H.core;

namespace Atomix.Kernel_H.plugs
{
    public static class BitConverter
    {
        [Plug("System_UInt32_System_BitConverter_ToUInt32_System_Byte____System_Int32_")]
        public static UInt32 ToUInt32(byte[] xData, int pos)
        {
            return (UInt32)(xData[pos] | (xData[pos + 1] << 8) | (xData[pos + 2] << 16) | (xData[pos + 3] << 24));
        }

        [Plug("System_Int32_System_BitConverter_ToInt32_System_Byte____System_Int32_")]
        public static Int32 ToInt32(byte[] xData, int pos)
        {
            return (Int32)(xData[pos] | (xData[pos + 1] << 8) | (xData[pos + 2] << 16) | (xData[pos + 3] << 24));
        }
    }
}
