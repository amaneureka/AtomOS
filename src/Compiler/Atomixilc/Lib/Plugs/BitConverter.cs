/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          System.BitConverter Plugs
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomixilc.Attributes;

namespace Atomixilc.Lib.Plugs
{
    internal unsafe static class BitConverterImpl
    {
        [Plug("System_UInt32_System_BitConverter_ToUInt32_System_Byte____System_Int32_")]
        internal static uint ToUInt32(byte* aData, int aIndex)
        {
            return *(uint*)(aData + 0x10 + aIndex);
        }

        [Plug("System_Int32_System_BitConverter_ToInt32_System_Byte____System_Int32_")]
        internal static int ToInt32(byte* aData, int aIndex)
        {
            return *(int*)(aData + 0x10 + aIndex);
        }

        [Plug("System_UInt16_System_BitConverter_ToUInt16_System_Byte____System_Int32_")]
        internal static ushort ToUInt16(byte* aData, int aIndex)
        {
            return *(ushort*)(aData + 0x10 + aIndex);
        }

        [Plug("System_Int16_System_BitConverter_ToInt16_System_Byte____System_Int32_")]
        internal static short ToInt16(byte* aData, int aIndex)
        {
            return *(short*)(aData + 0x10 + aIndex);
        }
    }
}
