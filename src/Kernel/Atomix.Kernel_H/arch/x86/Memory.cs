/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is strictly prohibited
*                   Proprietary and confidential
* PURPOSE:          Unsafe Memory support functions
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomixilc;
using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;

namespace Atomix.Kernel_H.Arch.x86
{
    internal static class Memory
    {
        /// <summary>
        /// Read 32 bit Memory at given address :)
        /// </summary>
        /// <param name="aAddress">Address of memory</param>
        /// <returns></returns>
        [Assembly(true)]
        internal static uint Read32(uint aAddress)
        {
            // Load address into EAX
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.EBP, SourceDisplacement = 0x8, SourceIndirect = true };
            // Read memory into EAX
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.EAX, SourceIndirect = true };

            return 0; // For c# error --> Don't make any sense for compiler
        }

        /// <summary>
        /// Read 16 bit Memory at given address :)
        /// </summary>
        /// <param name="aAddress">Address of memory</param>
        /// <returns></returns>
        [Assembly(true)]
        internal static ushort Read16(uint aAddress)
        {
            // Load address into EAX
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.EBP, SourceDisplacement = 0x8, SourceIndirect = true };
            // Read memory into EAX
            new Movzx { DestinationReg = Register.EAX, SourceReg = Register.EAX, SourceIndirect = true, Size = 16 };

            return 0; // For c# error --> Don't make any sense for compiler
        }

        /// <summary>
        /// Read 8 bit Memory at given address :)
        /// </summary>
        /// <param name="aAddress">Address of memory</param>
        /// <returns></returns>
        [Assembly(true)]
        internal static byte Read8(uint aAddress)
        {
            // Load address into EAX
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.EBP, SourceDisplacement = 0x8, SourceIndirect = true };
            // Read memory into EAX
            new Movzx { DestinationReg = Register.EAX, SourceReg = Register.EAX, SourceIndirect = true, Size = 8 };

            return 0; // For c# error --> Don't make any sense for compiler
        }

        /// <summary>
        /// Write 32 bit Memory at given address :)
        /// </summary>
        /// <param name="aAddress">Address of memory</param>
        /// <returns></returns>
        [Assembly(true)]
        internal static void Write32(uint aAddress, uint Value)
        {
            // Load address into EAX
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.EBP, SourceDisplacement = 0xC, SourceIndirect = true };
            // Load Value into EDX
            new Mov { DestinationReg = Register.EBX, SourceReg = Register.EBP, SourceDisplacement = 0x8, SourceIndirect = true };
            // Save value at mem Location
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.EBX, DestinationIndirect = true };
        }

        /// <summary>
        /// Write 16 bit Memory at given address :)
        /// </summary>
        /// <param name="aAddress">Address of memory</param>
        /// <returns></returns>
        [Assembly(true)]
        internal static void Write16(uint aAddress, ushort Value)
        {
            // Load address into EAX
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.EBP, SourceDisplacement = 0xC, SourceIndirect = true };
            // Load Value into EDX
            new Mov { DestinationReg = Register.EBX, SourceReg = Register.EBP, SourceDisplacement = 0x8, SourceIndirect = true };
            // Save value at mem Location
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.BX, DestinationIndirect = true, Size = 16 };
        }

        /// <summary>
        /// Write 8 bit Memory at given address :)
        /// </summary>
        /// <param name="aAddress">Address of memory</param>
        /// <returns></returns>
        [Assembly(true)]
        internal static void Write8(uint aAddress, byte Value)
        {
            // Load address into EAX
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.EBP, SourceDisplacement = 0xC, SourceIndirect = true };
            // Load Value into EDX
            new Mov { DestinationReg = Register.EBX, SourceReg = Register.EBP, SourceDisplacement = 0x8, SourceIndirect = true };
            // Save value at mem Location
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.BL, DestinationIndirect = true, Size = 8 };
        }

        [Assembly(true)]
        internal static void FastCopy(uint aDest, uint aSrc, uint aLen)
        {
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.EBP, SourceDisplacement = 8, SourceIndirect = true };
            new Mov { DestinationReg = Register.ESI, SourceReg = Register.EBP, SourceDisplacement = 12, SourceIndirect = true };
            new Mov { DestinationReg = Register.EDI, SourceReg = Register.EBP, SourceDisplacement = 16, SourceIndirect = true };

            new Mov { DestinationReg = Register.ECX, SourceReg = Register.EAX };
            new Shr { DestinationReg = Register.ECX, SourceRef = "0x2" };
            new Literal ("rep movsd");
            new Mov { DestinationReg = Register.ECX, SourceReg = Register.EAX };
            new And { DestinationReg = Register.ECX, SourceRef = "0x3" };
            new Literal ("rep movsb");
        }

        [Assembly(true)]
        internal static unsafe void FastClear(uint Address, uint Length)
        {
            new Mov { DestinationReg = Register.EBX, SourceReg = Register.EBP, SourceDisplacement = 0x8, SourceIndirect = true };
            new Mov { DestinationReg = Register.EDI, SourceReg = Register.EBP, SourceDisplacement = 0xC, SourceIndirect = true };

            new Xor { DestinationReg = Register.EAX, SourceReg = Register.EAX };
            new Mov { DestinationReg = Register.ECX, SourceReg = Register.EBX };
            new Shr { DestinationReg = Register.ECX, SourceRef = "0x2" };
            new Literal ("rep stosd");// Copy EAX to EDI
            new Mov { DestinationReg = Register.ECX, SourceReg = Register.EBX };
            new And { DestinationReg = Register.ECX, SourceRef = "0x3" };// Modulo by 4
            new Literal ("rep stosb");
        }
    }
}
