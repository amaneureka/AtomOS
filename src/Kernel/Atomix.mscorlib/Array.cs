using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;
using Atomix.Assembler;
using Atomix.Assembler.x86;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.mscorlib
{
    public static class ArrayImpl
    {
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
