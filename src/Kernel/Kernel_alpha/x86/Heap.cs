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
    public static class Heap
    {
        public static uint pointer = 0;
        private static uint _start = 0;
        [Label("Heap")]
        public static uint AllocateMem(uint aLength)
        {
            //This is temp.
            if (pointer == 0)
            {
                pointer = Native.EndOfKernel() + 0x200;
                _start = pointer;
            }

            if (pointer > _start + 0xA00000)
            {
                Console.WriteLine("Memory out of bound");
                while (true) ;
            }

            uint xResult = pointer;
            pointer += aLength;
            Memory.Clear(xResult, aLength);

            return xResult;
        }

        
    }
}
