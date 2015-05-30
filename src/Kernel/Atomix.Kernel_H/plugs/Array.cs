/* Copyright (C) Atomix Development, Inc - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Aman Priyadarshi <aman.eureka@gmail.com>, December 2014
 * 
 * Array.cs
 *      .Net Plug
 *      
 *      History:
 *          20-12-14    File Created    Aman Priyadarshi
 */

using System;

using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;

using Atomix.Assembler;
using Atomix.Assembler.x86;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.Kernel_H.plugs
{
    public static class ArrayImpl
    {
        [Assembly(0x8), Plug("System_Void_System_Runtime_CompilerServices_RuntimeHelpers_InitializeArray_System_Array__System_RuntimeFieldHandle_")]
        public static void InitializeArray(Array array, RuntimeFieldHandle fldHandle)
        {
            var xLabel = "System_Void_System_Runtime_CompilerServices_RuntimeHelpers_InitializeArray_System_Array__System_RuntimeFieldHandle_";

            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EDI, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 0xC });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ESI, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 0x8 });
            Core.AssemblerCode.Add(new Add { DestinationReg = Registers.EDI, SourceRef = "0x8" });
            Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EDI, DestinationIndirect = true });
            Core.AssemblerCode.Add(new Add { DestinationReg = Registers.EDI, SourceRef = "0x4" });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EDI, SourceIndirect = true });
            Core.AssemblerCode.Add(new Multiply { DestinationReg = Registers.ESP, DestinationIndirect = true, Size = 32 });
            Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.ECX });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, SourceReg = Registers.EAX });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceRef = "0x0" });
            Core.AssemblerCode.Add(new Add { DestinationReg = Registers.EDI, SourceRef = "0x4" });

            Core.AssemblerCode.Add(new Label(xLabel + ".StartLoop"));
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.DL, SourceReg = Registers.ESI, SourceIndirect = true, Size = 8 });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EDI, DestinationIndirect = true, SourceReg = Registers.DL, Size = 8 });
            Core.AssemblerCode.Add(new Add { DestinationReg = Registers.EAX, SourceRef = "0x1" });
            Core.AssemblerCode.Add(new Add { DestinationReg = Registers.ESI, SourceRef = "0x1" });
            Core.AssemblerCode.Add(new Add { DestinationReg = Registers.EDI, SourceRef = "0x1" });
            Core.AssemblerCode.Add(new Cmp { DestinationReg = Registers.EAX, SourceReg = Registers.ECX });
            Core.AssemblerCode.Add(new Jmp { Condition = ConditionalJumpEnum.JE, DestinationRef = xLabel + ".EndLoop" });
            Core.AssemblerCode.Add(new Jmp { DestinationRef = xLabel + ".StartLoop" });

            Core.AssemblerCode.Add(new Label(xLabel + ".EndLoop"));
        }

        [Plug("System_Void_System_Array_Copy_System_Array__System_Array__System_Int32_")]
        public static void Copy(Array SourceArray, Array DestinationArray, int len)
        {
            Copy(SourceArray, 0, DestinationArray, 0, len, false);
        }

        [Assembly(0x18), Plug("System_Void_System_Array_Copy_System_Array__System_Int32__System_Array__System_Int32__System_Int32__System_Boolean_")]
        public static void Copy(Array sourceArray, int sourceIndex, Array destinationArray, int destinationIndex, int length, bool reliable)
        {
            Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EBP, DestinationIndirect = true, DestinationDisplacement = 0x1C });
            Core.AssemblerCode.Add(new Add { DestinationReg = Registers.ESP, DestinationIndirect = true, SourceRef = "0xC", Size = 32 }); // pointer is at the element size
            Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EAX, SourceIndirect = true }); // element size
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 0x18 });
            Core.AssemblerCode.Add(new Multiply { DestinationReg = Registers.EBX });
            Core.AssemblerCode.Add(new Add { DestinationReg = Registers.EAX, SourceRef = "0x10" });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ESI, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 0x1C });
            Core.AssemblerCode.Add(new Add { DestinationReg = Registers.ESI, SourceReg = Registers.EAX }); // source ptr
            Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EBP, DestinationIndirect = true, DestinationDisplacement = 0x14 });
            Core.AssemblerCode.Add(new Add { DestinationReg = Registers.ESP, DestinationIndirect = true, SourceRef = "0xC", Size = 32 }); // pointer is at element size
            Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EAX, SourceIndirect = true });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 0x10 });
            Core.AssemblerCode.Add(new Multiply { DestinationReg = Registers.ECX });
            Core.AssemblerCode.Add(new Add { DestinationReg = Registers.EAX, SourceRef = "0x10" });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EDI, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 0x14 });
            Core.AssemblerCode.Add(new Add { DestinationReg = Registers.EDI, SourceReg = Registers.EAX });

            // calculate byte count to copy
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 0x14 });
            Core.AssemblerCode.Add(new Add { DestinationReg = Registers.EAX, SourceRef = "0xC" });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EAX, SourceIndirect = true });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EDX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 0xC });
            Core.AssemblerCode.Add(new Multiply { DestinationReg = Registers.EDX });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, SourceReg = Registers.EAX });
            Core.AssemblerCode.Add(new Literal("rep movsb"));
        }
    }
}
