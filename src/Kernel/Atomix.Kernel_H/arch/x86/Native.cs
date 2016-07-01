/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          x86 arch support functions
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomix.CompilerExt.Attributes;

using Atomix.Assembler;
using Atomix.Assembler.x86;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.Kernel_H.Arch.x86
{
    public static class Native
    {
        /// <summary>
        /// Clear Interrupts
        /// </summary>
        [Assembly(true)]
        public static void Cli()
        {
            AssemblyHelper.AssemblerCode.Add(new Cli ());
        }

        /// <summary>
        /// Enable Interrupts
        /// </summary>
        [Assembly(true)]
        public static void Sti()
        {
            AssemblyHelper.AssemblerCode.Add(new Sti ());
        }

        /// <summary>
        /// Halt The Processor
        /// </summary>
        [Assembly(true)]
        public static void Hlt()
        {
            AssemblyHelper.AssemblerCode.Add(new Literal ("hlt"));
        }

        /// <summary>
        /// Get Virtual Address of the object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Assembly(true)]
        public static uint GetAddress(object aObj)
        {
            return 0; // Only me and my compiler knows how it is working :P
        }

        /// <summary>
        /// Get Virtual Address of an array (addend reserved length)
        /// </summary>
        /// <param name="aArray"></param>
        /// <returns></returns>
        public static uint GetContentAddress(object aObj)
        {
            // 0x10 bytes are reserved for compiler specific work
            return (GetAddress(aObj) + 0x10);
        }

        /// <summary>
        /// End of kernel offset
        /// </summary>
        /// <returns></returns>
        [Assembly(true)]
        public static uint EndOfKernel()
        {
            // Just put Compiler_End location into return value
            AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBP, DestinationDisplacement = 0x8, DestinationIndirect = true, SourceRef = "Compiler_End" });

            return 0; // just for c# error
        }

        [Assembly(true)]
        public static uint CR2Register()
        {
            AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.CR2 });
            AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBP, DestinationDisplacement = 0x8, DestinationIndirect = true, SourceReg = Registers.EAX });
            return 0;
        }
    }
}
