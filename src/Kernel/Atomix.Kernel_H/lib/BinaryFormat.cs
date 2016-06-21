/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Binary Format extension functions
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

namespace Atomix.Kernel_H.lib
{
    public static class BinaryFormat
    {
        public static void SetByte(this byte[] aData, uint aOffset, byte aValue)
        {
            aData[aOffset] = aValue;
        }

        public static void SetUShort(this byte[] aData, uint aOffset, ushort aValue)
        {
            aData[aOffset + 0] = (byte)(aValue >> 0);
            aData[aOffset + 1] = (byte)(aValue >> 8);
        }

        public static void SetUInt(this byte[] aData, uint aOffset, uint aValue)
        {
            aData[aOffset + 0] = (byte)(aValue >> 0);
            aData[aOffset + 1] = (byte)(aValue >> 8);
            aData[aOffset + 2] = (byte)(aValue >> 16);
            aData[aOffset + 3] = (byte)(aValue >> 24);
        }

        public static void SetInt(this byte[] aData, uint aOffset, int aValue)
        {
            aData[aOffset + 0] = (byte)(aValue >> 0);
            aData[aOffset + 1] = (byte)(aValue >> 8);
            aData[aOffset + 2] = (byte)(aValue >> 16);
            aData[aOffset + 3] = (byte)(aValue >> 24);
        }

        public static void SetShort(this byte[] aData, uint aOffset, short aValue)
        {
            aData[aOffset + 0] = (byte)(aValue >> 0);
            aData[aOffset + 1] = (byte)(aValue >> 8);
        }

        public static void SetStringASCII(this byte[] aData, uint aOffset, string aValue, int alength = -1)
        {
            int length = (alength == -1 ?  aValue.Length : alength);
            for (int i = 0; i < length; i++)
                aData[aOffset + i] = (byte)aValue[i];
        }

        public static void SetStringUnicode(this byte[] aData, uint aOffset, string aValue, int alength = -1)
        {
            int length = (alength == -1 ? aValue.Length : alength);
            for (int i = 0; i < length; i++)
            {
                aData[aOffset + (i << 1) + 0] = (byte)(aValue[i] >> 0);
                aData[aOffset + (i << 1) + 1] = (byte)(aValue[i] >> 8);
            }
        }
    }
}
