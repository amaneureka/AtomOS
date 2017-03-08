/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          System.Array Plugs
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;

namespace Atomixilc.Lib.Plugs
{
    internal static class ArrayImpl
    {
        [Plug("System_Void_System_Array_Copy_System_Array__System_Array__System_Int32_", Architecture.x86)]
        internal static void CopyHelper(Array aSourceArray, Array aDestinationArray, int aLength)
        {
            Copy(aSourceArray, 0, aDestinationArray, 0, aLength, false);
        }

        [Plug("System_Void_System_Array_Copy_System_Array__System_Int32__System_Array__System_Int32__System_Int32_", Architecture.x86)]
        internal static void CopyHelper(Array aSourceArray, int aSourceIndex, Array aDestinationArray, int aDestinationIndex, int aLength)
        {
            Copy(aSourceArray, aSourceIndex, aDestinationArray, aDestinationIndex, aLength, false);
        }

        [Assembly(true), Plug("System_Void_System_Array_Copy_System_Array__System_Int32__System_Array__System_Int32__System_Int32__System_Boolean_", Architecture.x86)]
        internal static void Copy(Array aSourceArray, int aSourceIndex, Array aDestinationArray, int aDestinationIndex, int aLength, bool aReliable)
        {
            // Destination
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.EBP, SourceDisplacement = 0x14, SourceIndirect = true };
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.EAX, SourceDisplacement = 0xC, SourceIndirect = true };
            new Mov { DestinationReg = Register.EBX, SourceReg = Register.EBP, SourceDisplacement = 0x10, SourceIndirect = true };
            new Mul { DestinationReg = Register.EBX };
            new Add { DestinationReg = Register.EAX, SourceReg = Register.EBP, SourceDisplacement = 0x14, SourceIndirect = true };
            new Add { DestinationReg = Register.EAX, SourceRef = "0x10" };
            new Mov { DestinationReg = Register.EDI, SourceReg = Register.EAX };

            // Source
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.EBP, SourceDisplacement = 0x1C, SourceIndirect = true };
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.EAX, SourceDisplacement = 0xC, SourceIndirect = true };
            new Mov { DestinationReg = Register.EBX, SourceReg = Register.EBP, SourceDisplacement = 0x18, SourceIndirect = true };
            new Mul { DestinationReg = Register.EBX };
            new Add { DestinationReg = Register.EAX, SourceReg = Register.EBP, SourceDisplacement = 0x1C, SourceIndirect = true };
            new Add { DestinationReg = Register.EAX, SourceRef = "0x10" };
            new Mov { DestinationReg = Register.ESI, SourceReg = Register.EAX };

            // copy
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.EBP, SourceDisplacement = 0x14, SourceIndirect = true };
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.EAX, SourceDisplacement = 0xC, SourceIndirect = true };
            new Mov { DestinationReg = Register.EDX, SourceReg = Register.EBP, SourceDisplacement = 0xC, SourceIndirect = true };
            new Mul { DestinationReg = Register.EDX };
            new Mov { DestinationReg = Register.ECX, SourceReg = Register.EAX };
            new Shr { DestinationReg = Register.ECX, SourceRef = "0x2" };
            new And { DestinationReg = Register.EAX, SourceRef = "0x3" };
            new Literal("rep movsd");
            new Mov { DestinationReg = Register.ECX, SourceReg = Register.EAX };
            new Literal("rep movsb");
        }
    }
}
