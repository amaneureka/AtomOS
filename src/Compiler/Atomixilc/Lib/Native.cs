/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Application architecture native support functions
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;

namespace Atomixilc.Lib
{
    public static class Native
    {
        /// <summary>
        /// Clear Interrupts
        /// </summary>
        [NoException]
        [Assembly(false)]
        public static void Cli()
        {
            new Cli();
            new Ret { Offset = 0x0 };
        }

        /// <summary>
        /// Enable Interrupts
        /// </summary>
        [NoException]
        [Assembly(false)]
        public static void Sti()
        {
            new Sti();
            new Ret { Offset = 0x0 };
        }

        /// <summary>
        /// Halt The Processor
        /// </summary>
        [NoException]
        [Assembly(false)]
        public static void Hlt()
        {
            new Literal("hlt");
            new Ret { Offset = 0x0 };
        }

        /// <summary>
        /// Get Virtual Address of the object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [NoException]
        [Assembly(false)]
        public static uint GetAddress(object aObj)
        {
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.ESP, SourceDisplacement = 0x4, SourceIndirect = true };
            new Ret { Offset = 0x4 };

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
        /// Get Invokable method address from Action Delegate
        /// </summary>
        /// <param name="aDelegate"></param>
        /// <returns></returns>
        [NoException]
        [Assembly(false)]
        public static uint InvokableAddress(this Delegate aDelegate)
        {
            // Compiler.cs : ProcessDelegate(MethodBase xMethod);
            // [aDelegate + 0xC] := Address Field
            new Mov
            {
                DestinationReg = Register.EAX,
                SourceReg = Register.ESP,
                SourceDisplacement = 0x4,
                SourceIndirect = true
            };

            new Mov { DestinationReg = Register.EAX,  SourceReg = Register.EAX, SourceDisplacement = 0xC, SourceIndirect = true };
            new Ret { Offset = 0x4 };

            return 0;
        }

        /// <summary>
        /// End of kernel offset
        /// </summary>
        /// <returns></returns>
        [NoException]
        [Assembly(false)]
        public static uint EndOfKernel()
        {
            // Just put Compiler_End location into return value
            new Mov { DestinationReg = Register.EAX, SourceRef = "Compiler_End" };
            new Ret { Offset = 0x0 };

            return 0; // just for c# error
        }

        [NoException]
        [Assembly(false)]
        public static uint CR2Register()
        {
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.CR2 };
            new Ret { Offset = 0x0 };

            return 0;
        }

        [NoException]
        [Assembly(false)]
        public static uint AtomicExchange(ref uint aLock, uint val)
        {
            new Mov { DestinationReg = Register.EBX, SourceReg = Register.ESP, SourceDisplacement = 0x8, SourceIndirect = true };
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.ESP, SourceDisplacement = 0x8, SourceIndirect = true };
            new Literal("xchg dword [EBX], EAX");
            new Ret { Offset = 0x8 };

            return 0;
        }
    }
}
