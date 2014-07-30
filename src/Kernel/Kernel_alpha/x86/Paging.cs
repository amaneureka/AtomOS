using System;
using System.Collections.Generic;
using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;
using Atomix.Assembler;
using Atomix.Assembler.x86;
using Core = Atomix.Assembler.AssemblyHelper;
using Kernel_alpha.x86.Intrinsic;

namespace Kernel_alpha.x86
{
    public static unsafe class Paging
    {
        private static uint PageDirectory;

        public static void Setup()
        {
            PageDirectory = (Native.EndOfKernel() & 0xFFFFF000) + 0x1000;

            //set each entry to not present
            for (int i = 0; i < 1024; i++)
            {
                //attribute: supervisor level, read/write, not present.
                *(UInt32*)(PageDirectory + i) = 0x0 | 0x2;
            }

            //First Page table --> Map 4MB of memory
            UInt32* Page_Table = (UInt32*)(PageDirectory + 1024);

            uint Address = 0;
            for (int i = 0; i < 1024; i++)
            {
                Page_Table[i] = Address | 0x3; // attributes: supervisor level, read/write, present.
                Address += 4096;//advance the address to the next page boundary
            }

            // attributes: supervisor level, read/write, present
            *(UInt32*)(PageDirectory + 0) = *(UInt32*)Page_Table | 0x3;

            EnablePaging(PageDirectory);
        }

        [Assembly(0x4)]
        private static void EnablePaging(uint PageDirectory)
        {
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBX, SourceReg = Registers.EBP, SourceDisplacement = 0x8, SourceIndirect = true });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.CR3,  SourceReg = Registers.EBX });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.CR0 });
            Core.AssemblerCode.Add(new Or { DestinationReg = Registers.EAX, SourceRef = "0x80000000" });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.CR0, SourceReg = Registers.EAX });
        }
    }
}
