/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is strictly prohibited
*                   Proprietary and confidential
* PURPOSE:          File Contains various mscorlib plug belongs to BitConverter class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomix.CompilerExt.Attributes;

namespace Atomix.Kernel_H.plugs
{
    public static class BitConverter
    {
        /* 
         * Based on the assumption that we are on arch. which supports Little Endian.
         */

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

        [Plug("System_UInt16_System_BitConverter_ToUInt16_System_Byte____System_Int32_")]
        public static UInt16 ToUInt16(byte[] xData, int pos)
        {
            return (UInt16)(xData[pos] | (xData[pos + 1] << 8));
        }

        [Plug("System_Int16_System_BitConverter_ToInt16_System_Byte____System_Int32_")]
        public static Int16 ToInt16(byte[] xData, int pos)
        {
            return (Int16)(xData[pos] | (xData[pos + 1] << 8));
        }
    }
}
