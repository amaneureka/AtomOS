using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atomix.CompilerExt;
using Kernel_alpha.x86.Intrinsic;
using Atomix.CompilerExt.Attributes;
using Atomix.Assembler;
using Atomix.Assembler.x86;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Kernel_alpha.x86
{
    public static class Memory
    {
        /*
        public static void Clear(uint Address, uint ByteCount)
        {
            uint Mem4 = ByteCount / 4;
            for (uint i = Address; i < Address + (4 * Mem4); i+=4)
                Native.Write32(i, 0x0);

            uint Rest = ByteCount % 4;
            for (uint i = Address + (4 * Mem4); i < Address + ByteCount; i++)
                Native.Write8(i, 0x0);
        }*/

        [Assembly(0x8), Plug("Clear_Memory")]
        public static void Clear(uint Address, uint Length)
        {
            Core.AssemblerCode.Add(new Literal("cld"));
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EDI, SourceReg = Registers.EBP, SourceDisplacement = 0x12, SourceIndirect = true });
            Core.AssemblerCode.Add(new Xor { DestinationReg = Registers.EAX, SourceReg = Registers.EAX });
            Core.AssemblerCode.Add(new Shr { DestinationReg = Registers.ECX, SourceRef = "0x1" });
            Core.AssemblerCode.Add(new Jmp { Condition = ConditionalJumpEnum.JNB, DestinationRef = "Clear_Memory.Step2" });
            Core.AssemblerCode.Add(new Literal("stosb"));
            Core.AssemblerCode.Add(new Label("Clear_Memory.Step2"));
            Core.AssemblerCode.Add(new Shr { DestinationReg = Registers.ECX, SourceRef = "0x1" });
            Core.AssemblerCode.Add(new Jmp { Condition = ConditionalJumpEnum.JNB, DestinationRef = "Clear_Memory.Step3" });
            Core.AssemblerCode.Add(new Literal("stosw"));
            Core.AssemblerCode.Add(new Label("Clear_Memory.Step3"));
            Core.AssemblerCode.Add(new Literal("rep stosb"));
        }
    }
}
