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

namespace Atomix.Kernel_H.arch.x86
{
    public static class Native
    {
        /// <summary>
        /// Clear Interrupts
        /// </summary>
        [Assembly(0x0)]
        public static void Cli()
        {
            Core.AssemblerCode.Add(new Cli());
        }

        /// <summary>
        /// Enable Interrupts
        /// </summary>
        [Assembly(0x0)]
        public static void Sti()
        {
            Core.AssemblerCode.Add(new Sti());
        }

        /// <summary>
        /// Halt The Processor
        /// </summary>
        [Assembly(0x0)]
        public static void Hlt()
        {
            Core.AssemblerCode.Add(new Literal("hlt"));
        }

        /// <summary>
        /// Get Virtual Address of the object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Assembly(0x0)]
        public static unsafe uint GetAddress(object obj)
        {
            return 0;//Only me and my compiler knows how it is working :P
        }

        /// <summary>
        /// End of kernel offset
        /// </summary>
        /// <returns></returns>
        [Assembly(0x0)]
        public static uint EndOfKernel()
        {
            //Just put Compiler_End location into return value
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBP, DestinationDisplacement = 0x8, DestinationIndirect = true, SourceRef = "Compiler_End" });

            return 0; //just for c# error
        }

        [Assembly(0x0)]
        public static uint CR2Register()
        {
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.CR2 });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBP, DestinationDisplacement = 0x8, DestinationIndirect = true, SourceReg = Registers.EAX });
            return 0;
        }
    }
}
