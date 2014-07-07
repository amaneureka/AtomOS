using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;
using Atomix.Assembler;
using Atomix.Assembler.x86;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Kernel_alpha.x86.Intrinsic
{
    public static class IRQ
    {
        /// <summary>
        /// Fire Timer Interrupt
        /// </summary>
        [Assembly(0x0)]
        public static void Timer()
        {
            Core.AssemblerCode.Add(new Literal("int 0x20"));
        }

        /// <summary>
        /// Fire Keyboard Interrupt
        /// </summary>
        [Assembly(0x0)]
        public static void Keyboard()
        {
            Core.AssemblerCode.Add(new Literal("int 0x21"));
        }

        /// <summary>
        /// Fire Mouse Interrupt
        /// </summary>
        [Assembly (0x0)]
        public static void Mouse ()
        {
            Core.AssemblerCode.Add (new Literal ("int 0x12"));
        }
    }
}
