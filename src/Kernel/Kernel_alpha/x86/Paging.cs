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
        public static void Setup()
        {
            var PageDirectory = (Native.EndOfKernel() & 0xFFFFF000) + 0x1000;

            UInt32* PageDir = (UInt32*)PageDirectory;

            for (int i = 0; i < 1024; i++)
            {
                //attribute: supervisor level, read/write, not present.
                PageDir[i] = 0x0 | 0x2;
            }

            //First Page table --> Map 4MB of memory
            UInt32* Page_Table = (UInt32*)(PageDirectory + 1024);

            uint Address = 0;
            for (int i = 0; i < 1024; i++)
            {
                Page_Table[i] = Address | 0x3; // attributes: supervisor level, read/write, present.
                Address += 4096;//advance the address to the next page boundary
            }
            
            PageDir[0] = PageDirectory + 1024;
            PageDir[0] |= 0x1;// attributes: supervisor level, read/write, present

            EnablePaging(PageDirectory);
        }

        [Assembly(0x4)]
        private static void EnablePaging(uint PageDirectory)
        {
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceDisplacement = 0x8, SourceIndirect = true });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.CR3,  SourceReg = Registers.EBX });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBX, SourceReg = Registers.CR0 });
            Core.AssemblerCode.Add(new Or { DestinationReg = Registers.EBX, SourceRef = "0x80000000" });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.CR0, SourceReg = Registers.EBX });
        }
    }
}
