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
        private static uint pointer;
        private static uint _start;

        [Label("Heap")]
        public static uint AllocateMem(uint aLength)
        {
            //This is temp.
            if (_start == 0)
                _start = Native.EndOfKernel();

            uint xResult = pointer;
            pointer += aLength;
            Memory.Clear(xResult, aLength);

            return xResult;
        }

        
    }
}
