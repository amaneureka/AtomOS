/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          File Contains various mscorlib plug belongs to Array class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomixilc;
using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;

namespace Atomix.Kernel_H.plugs
{
    internal static class ArrayImpl
    {
        [Assembly(true), Plug("System_Void_System_Runtime_CompilerServices_RuntimeHelpers_InitializeArray_System_Array__System_RuntimeFieldHandle_", Architecture.x86)]
        internal static void InitializeArray(Array aArray, RuntimeFieldHandle aHandler)
        {
            new Mov { DestinationReg = Register.EDI, SourceReg = Register.EBP, SourceIndirect = true, SourceDisplacement = 0xC };
            new Mov { DestinationReg = Register.ESI, SourceReg = Register.EBP, SourceIndirect = true, SourceDisplacement = 0x8 };

            new Add { DestinationReg = Register.EDI, SourceRef = "0x8" };
            new Push { DestinationReg = Register.EDI, DestinationIndirect = true };

            new Add { DestinationReg = Register.EDI, SourceRef = "0x4" };
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.EDI, SourceIndirect = true };
            new Mul { DestinationReg = Register.ESP, DestinationIndirect = true };
            new Pop { DestinationReg = Register.ECX };
            new Mov { DestinationReg = Register.ECX, SourceReg = Register.EAX };
            new Add { DestinationReg = Register.EDI, SourceRef = "0x4" };

            new Shr { DestinationReg = Register.ECX, SourceRef = "0x2" };
            new And { DestinationReg = Register.EAX, SourceRef = "0x3" };
            new Literal ("rep movsd");
            new Mov { DestinationReg = Register.ECX, SourceReg = Register.EAX };
            new Literal ("rep movsb");
        }

        /// <summary>
        /// Handles Array.Copy Function
        /// </summary>
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
            new Literal ("rep movsd");
            new Mov { DestinationReg = Register.ECX, SourceReg = Register.EAX };
            new Literal ("rep movsb");
        }
   }
}
