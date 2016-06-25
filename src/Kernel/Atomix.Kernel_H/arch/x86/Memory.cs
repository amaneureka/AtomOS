﻿/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is strictly prohibited
*                   Proprietary and confidential
* PURPOSE:          Unsafe Memory support functions
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomix.CompilerExt.Attributes;

using Atomix.Assembler;
using Atomix.Assembler.x86;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.Kernel_H.arch.x86
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
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceDisplacement = 0x8, SourceIndirect = true });
            // Read memory into EAX
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBX, SourceReg = Registers.EAX, SourceIndirect = true });
            // Save read out value into stack
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBP, SourceReg = Registers.EBX, DestinationDisplacement = 0x8, DestinationIndirect = true });

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
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceDisplacement = 0x8, SourceIndirect = true });
            // Clean EBX Register
            Core.AssemblerCode.Add(new Xor { DestinationReg = Registers.EBX, SourceReg = Registers.EBX });
            // Read memory into EAX
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBX, SourceReg = Registers.EAX, SourceIndirect = true });
            // Save read out value into stack
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBP, SourceReg = Registers.BX, DestinationDisplacement = 0x8, DestinationIndirect = true, Size = 16 });

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
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceDisplacement = 0x8, SourceIndirect = true });
            // Clean EBX Register
            Core.AssemblerCode.Add(new Xor { DestinationReg = Registers.EBX, SourceReg = Registers.EBX });
            // Read memory into EAX
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBX, SourceReg = Registers.EAX, SourceIndirect = true });
            // Save read out value into stack
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBP, SourceReg = Registers.BL, DestinationDisplacement = 0x8, DestinationIndirect = true, Size = 8 });

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
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceDisplacement = 0xC, SourceIndirect = true });
            // Load Value into EDX
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBX, SourceReg = Registers.EBP, SourceDisplacement = 0x8, SourceIndirect = true });
            // Save value at mem Location
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBX, DestinationIndirect = true });
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
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceDisplacement = 0xC, SourceIndirect = true });
            // Load Value into EDX
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBX, SourceReg = Registers.EBP, SourceDisplacement = 0x8, SourceIndirect = true });
            // Save value at mem Location
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.BX, DestinationIndirect = true, Size = 16 });
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
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceDisplacement = 0xC, SourceIndirect = true });
            // Load Value into EDX
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBX, SourceReg = Registers.EBP, SourceDisplacement = 0x8, SourceIndirect = true });
            // Save value at mem Location
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.BL, DestinationIndirect = true, Size = 8 });
        }

        [Assembly(true)]
        internal static void FastCopy(uint aDest, uint aSrc, uint aLen)
        {
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceDisplacement = 8, SourceIndirect = true });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ESI, SourceReg = Registers.EBP, SourceDisplacement = 12, SourceIndirect = true });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EDI, SourceReg = Registers.EBP, SourceDisplacement = 16, SourceIndirect = true });

            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, SourceReg = Registers.EAX });
            Core.AssemblerCode.Add(new ShiftRight { DestinationReg = Registers.ECX, SourceRef = "0x2" });
            Core.AssemblerCode.Add(new Literal("rep movsd"));
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, SourceReg = Registers.EAX });
            Core.AssemblerCode.Add(new And { DestinationReg = Registers.ECX, SourceRef = "0x3" });
            Core.AssemblerCode.Add(new Literal("rep movsb"));
        }

        [Assembly(true)]
        internal static unsafe void FastClear(uint Address, uint Length)
        {
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBX, SourceReg = Registers.EBP, SourceDisplacement = 0x8, SourceIndirect = true });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EDI, SourceReg = Registers.EBP, SourceDisplacement = 0xC, SourceIndirect = true });

            Core.AssemblerCode.Add(new Xor { DestinationReg = Registers.EAX, SourceReg = Registers.EAX });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, SourceReg = Registers.EBX });
            Core.AssemblerCode.Add(new ShiftRight { DestinationReg = Registers.ECX, SourceRef = "0x2" });
            Core.AssemblerCode.Add(new Literal("rep stosd"));// Copy EAX to EDI
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, SourceReg = Registers.EBX });
            Core.AssemblerCode.Add(new And { DestinationReg = Registers.ECX, SourceRef = "0x3" });// Modulo by 4
            Core.AssemblerCode.Add(new Literal("rep stosb"));
        }
    }
}
