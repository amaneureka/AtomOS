/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          x86 arch support functions
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomixilc;
using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;

namespace Atomix.Kernel_H.Arch.x86
{
    internal static class Native
    {
        /// <summary>
        /// Clear Interrupts
        /// </summary>
        [Assembly(true)]
        internal static void Cli()
        {
            new Cli ();
        }

        /// <summary>
        /// Enable Interrupts
        /// </summary>
        [Assembly(true)]
        internal static void Sti()
        {
            new Sti ();
        }

        /// <summary>
        /// Halt The Processor
        /// </summary>
        [Assembly(true)]
        internal static void Hlt()
        {
            new Literal ("hlt");
        }

        /// <summary>
        /// Get Virtual Address of the object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Assembly(true)]
        internal static uint GetAddress(object aObj)
        {
            new Push { DestinationReg = Register.EBP, DestinationDisplacement = 0x8, DestinationIndirect = true };

            return 0; // Only me and my compiler knows how it is working :P
        }

        /// <summary>
        /// Get Virtual Address of an array (addend reserved length)
        /// </summary>
        /// <param name="aArray"></param>
        /// <returns></returns>
        internal static uint GetContentAddress(object aObj)
        {
            // 0x10 bytes are reserved for compiler specific work
            return (GetAddress(aObj) + 0x10);
        }

        /// <summary>
        /// Get Invokable method address from Action Delegate
        /// </summary>
        /// <param name="aDelegate"></param>
        /// <returns></returns>
        [Assembly(true)]
        internal static uint InvokableAddress(this Delegate aDelegate)
        {
            // Compiler.cs : ProcessDelegate(MethodBase xMethod);
            // [aDelegate + 0xC] := Address Field
            new Mov
            {
                DestinationReg = Register.EAX,
                SourceReg = Register.EBP,
                SourceDisplacement = 0x8,
                SourceIndirect = true
            };

            new Push { DestinationReg = Register.EAX, DestinationDisplacement = 0xC, DestinationIndirect = true };
            return 0;
        }

        /// <summary>
        /// End of kernel offset
        /// </summary>
        /// <returns></returns>
        [Assembly(true)]
        internal static uint EndOfKernel()
        {
            // Just put Compiler_End location into return value
            new Push { DestinationRef = "Compiler_End" };
            return 0; // just for c# error
        }

        [Assembly(true)]
        internal static uint CR2Register()
        {
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.CR2 };
            new Push { DestinationReg = Register.EAX };
            return 0;
        }
    }
}
