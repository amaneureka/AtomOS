/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
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
        
        [NoException]
        [Assembly(false)]
        public static uint GetDataOffset(this string aStr)
        {
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.ESP, SourceDisplacement = 0x4, SourceIndirect = true };
            new Add { DestinationReg = Register.EAX, SourceRef = "0x10" };
            new Ret { Offset = 0x4 };

            return 0;
        }

        [NoException]
        [Assembly(false)]
        public static uint GetDataOffset(this Array aArray)
        {
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.ESP, SourceDisplacement = 0x4, SourceIndirect = true };
            new Add { DestinationReg = Register.EAX, SourceRef = "0x10" };
            new Ret { Offset = 0x4 };

            return 0;
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
