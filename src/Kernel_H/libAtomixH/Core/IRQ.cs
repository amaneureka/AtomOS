using System;

using Atomix.CompilerExt.Attributes;
using Atomix.Assembler;
using asm = Atomix.Assembler.AssemblyHelper;

namespace libAtomixH.Core
{
    public static class IRQ
    {
        /// <summary>
        /// Fire Timer Interrupt
        /// </summary>
        [Assembly (0x0)]
        public static void Timer ()
        {
            asm.AssemblerCode.Add (new Literal ("int 0x20"));
        }

        /// <summary>
        /// Fire Keyboard Interrupt
        /// </summary>
        [Assembly (0x0)]
        public static void Keyboard ()
        {
            asm.AssemblerCode.Add (new Literal ("int 0x21"));
        }

        /// <summary>
        /// Fire Mouse Interrupt
        /// </summary>
        [Assembly (0x0)]
        public static void Mouse ()
        {
            asm.AssemblerCode.Add (new Literal ("int 0x2C"));
        }
    }
}
