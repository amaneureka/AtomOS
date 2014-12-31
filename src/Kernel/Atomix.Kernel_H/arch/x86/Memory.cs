/* Copyright (C) Atomix Development, Inc - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Aman Priyadarshi <aman.eureka@gmail.com>, December 2014
 * 
 * Memory.cs
 *      Native Plugs to read write memory in RAM
 *      
 *      History:
 *          19-12-14    File Created    Aman Priyadarshi
 */

using System;
using System.Collections.Generic;

using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;

using Atomix.Assembler;
using Atomix.Assembler.x86;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.Kernel_H.arch.x86
{
    public static class Memory
    {
        /// <summary>
        /// Read 32 bit Memory at given address :)
        /// </summary>
        /// <param name="aAddress">Address of memory</param>
        /// <returns></returns>
        [Assembly(0x0)]
        public static uint Read32(uint aAddress)
        {
            //Load address into EAX
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceDisplacement = 0x8, SourceIndirect = true });
            //Read memory into EAX
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBX, SourceReg = Registers.EAX, SourceIndirect = true });
            //Save read out value into stack
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBP, SourceReg = Registers.EBX, DestinationDisplacement = 0x8, DestinationIndirect = true });

            return 0; //For c# error --> Don't make any sense for compiler
        }

        /// <summary>
        /// Read 16 bit Memory at given address :)
        /// </summary>
        /// <param name="aAddress">Address of memory</param>
        /// <returns></returns>
        [Assembly(0x0)]
        public static ushort Read16(uint aAddress)
        {
            //Load address into EAX
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceDisplacement = 0x8, SourceIndirect = true });
            //Clean EBX Register
            Core.AssemblerCode.Add(new Xor { DestinationReg = Registers.EBX, SourceReg = Registers.EBX });
            //Read memory into EAX
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBX, SourceReg = Registers.EAX, SourceIndirect = true });
            //Save read out value into stack
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBP, SourceReg = Registers.BX, DestinationDisplacement = 0x8, DestinationIndirect = true, Size = 16 });

            return 0; //For c# error --> Don't make any sense for compiler
        }

        /// <summary>
        /// Read 8 bit Memory at given address :)
        /// </summary>
        /// <param name="aAddress">Address of memory</param>
        /// <returns></returns>
        [Assembly(0x0)]
        public static byte Read8(uint aAddress)
        {
            //Load address into EAX
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceDisplacement = 0x8, SourceIndirect = true });
            //Clean EBX Register
            Core.AssemblerCode.Add(new Xor { DestinationReg = Registers.EBX, SourceReg = Registers.EBX });
            //Read memory into EAX
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBX, SourceReg = Registers.EAX, SourceIndirect = true });
            //Save read out value into stack
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBP, SourceReg = Registers.BL, DestinationDisplacement = 0x8, DestinationIndirect = true, Size = 8 });

            return 0; //For c# error --> Don't make any sense for compiler
        }

        /// <summary>
        /// Write 32 bit Memory at given address :)
        /// </summary>
        /// <param name="aAddress">Address of memory</param>
        /// <returns></returns>
        [Assembly(0x8)]
        public static void Write32(uint aAddress, uint Value)
        {
            //Load address into EAX
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceDisplacement = 0xC, SourceIndirect = true });
            //Load Value into EDX
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBX, SourceReg = Registers.EBP, SourceDisplacement = 0x8, SourceIndirect = true });
            //Save value at mem Location
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBX, DestinationIndirect = true });
        }

        /// <summary>
        /// Write 16 bit Memory at given address :)
        /// </summary>
        /// <param name="aAddress">Address of memory</param>
        /// <returns></returns>
        [Assembly(0x8)]
        public static void Write16(uint aAddress, ushort Value)
        {
            //Load address into EAX
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceDisplacement = 0xC, SourceIndirect = true });
            //Load Value into EDX
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBX, SourceReg = Registers.EBP, SourceDisplacement = 0x8, SourceIndirect = true });
            //Save value at mem Location
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.BX, DestinationIndirect = true, Size = 16 });
        }

        /// <summary>
        /// Write 8 bit Memory at given address :)
        /// </summary>
        /// <param name="aAddress">Address of memory</param>
        /// <returns></returns>
        [Assembly(0x8)]
        public static void Write8(uint aAddress, byte Value)
        {
            //Load address into EAX
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceDisplacement = 0xC, SourceIndirect = true });
            //Load Value into EDX
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBX, SourceReg = Registers.EBP, SourceDisplacement = 0x8, SourceIndirect = true });
            //Save value at mem Location
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.BL, DestinationIndirect = true, Size = 8 });
        }
    }
}
