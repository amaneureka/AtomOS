/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          File Contains various mscorlib plug belongs to Array class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomix.CompilerExt.Attributes;

using Atomix.Assembler;
using Atomix.Assembler.x86;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.Kernel_H.plugs
{
    internal static class ArrayImpl
    {
        [Assembly(true), Plug("System_Void_System_Runtime_CompilerServices_RuntimeHelpers_InitializeArray_System_Array__System_RuntimeFieldHandle_")]
        internal static void InitializeArray(Array aArray, RuntimeFieldHandle aHandler)
        {
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EDI, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 0xC });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ESI, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 0x8 });

            Core.AssemblerCode.Add(new Add { DestinationReg = Registers.EDI, SourceRef = "0x8" });
            Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EDI, DestinationIndirect = true });

            Core.AssemblerCode.Add(new Add { DestinationReg = Registers.EDI, SourceRef = "0x4" });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EDI, SourceIndirect = true });
            Core.AssemblerCode.Add(new Multiply { DestinationReg = Registers.ESP, DestinationIndirect = true });
            Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.ECX });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, SourceReg = Registers.EAX });
            Core.AssemblerCode.Add(new Add { DestinationReg = Registers.EDI, SourceRef = "0x4" });

            Core.AssemblerCode.Add(new ShiftRight { DestinationReg = Registers.ECX, SourceRef = "0x2" });
            Core.AssemblerCode.Add(new And { DestinationReg = Registers.EAX, SourceRef = "0x3" });
            Core.AssemblerCode.Add(new Literal("rep movsd"));
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, SourceReg = Registers.EAX });
            Core.AssemblerCode.Add(new Literal("rep movsb"));
        }

        /// <summary>
        /// Handles Array.Copy Function
        /// </summary>
        [Plug("System_Void_System_Array_Copy_System_Array__System_Array__System_Int32_")]
        internal static void CopyHelper(Array aSourceArray, Array aDestinationArray, int aLength)
        {
            Copy(aSourceArray, 0, aDestinationArray, 0, aLength, false);
        }

		[Plug("System_Void_System_Array_Copy_System_Array__System_Int32__System_Array__System_Int32__System_Int32_")]
        internal static void CopyHelper(Array aSourceArray, int aSourceIndex, Array aDestinationArray, int aDestinationIndex, int aLength)
        {
            Copy(aSourceArray, aSourceIndex, aDestinationArray, aDestinationIndex, aLength, false);
        }

        [Assembly(true), Plug("System_Void_System_Array_Copy_System_Array__System_Int32__System_Array__System_Int32__System_Int32__System_Boolean_")]
        internal static void Copy(Array aSourceArray, int aSourceIndex, Array aDestinationArray, int aDestinationIndex, int aLength, bool aReliable)
        {
            // Destination
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceDisplacement = 0x14, SourceIndirect = true });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EAX, SourceDisplacement = 0xC, SourceIndirect = true });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBX, SourceReg = Registers.EBP, SourceDisplacement = 0x10, SourceIndirect = true });
            Core.AssemblerCode.Add(new Multiply { DestinationReg = Registers.EBX });
            Core.AssemblerCode.Add(new Add { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceDisplacement = 0x14, SourceIndirect = true });
            Core.AssemblerCode.Add(new Add { DestinationReg = Registers.EAX, SourceRef = "0x10" });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EDI, SourceReg = Registers.EAX });

            // Source
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceDisplacement = 0x1C, SourceIndirect = true });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EAX, SourceDisplacement = 0xC, SourceIndirect = true });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBX, SourceReg = Registers.EBP, SourceDisplacement = 0x18, SourceIndirect = true });
            Core.AssemblerCode.Add(new Multiply { DestinationReg = Registers.EBX });
            Core.AssemblerCode.Add(new Add { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceDisplacement = 0x1C, SourceIndirect = true });
            Core.AssemblerCode.Add(new Add { DestinationReg = Registers.EAX, SourceRef = "0x10" });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ESI, SourceReg = Registers.EAX });

            // copy
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceDisplacement = 0x14, SourceIndirect = true });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EAX, SourceDisplacement = 0xC, SourceIndirect = true });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EDX, SourceReg = Registers.EBP, SourceDisplacement = 0xC, SourceIndirect = true });
            Core.AssemblerCode.Add(new Multiply { DestinationReg = Registers.EDX });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, SourceReg = Registers.EAX });
            Core.AssemblerCode.Add(new ShiftRight { DestinationReg = Registers.ECX, SourceRef = "0x2" });
            Core.AssemblerCode.Add(new And { DestinationReg = Registers.EAX, SourceRef = "0x3" });
            Core.AssemblerCode.Add(new Literal("rep movsd"));
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, SourceReg = Registers.EAX });
            Core.AssemblerCode.Add(new Literal("rep movsb"));
        }
   }
}
