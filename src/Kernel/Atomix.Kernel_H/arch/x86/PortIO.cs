/* Copyright (C) Atomix Development, Inc - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Aman Priyadarshi <aman.eureka@gmail.com>, December 2014
 * 
 * PortIO.cs
 *      I/O Port In out Functions
 *      
 *      History:
 *          20-12-14    File Created    Aman Priyadarshi
 */

using System;

using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;

using Atomix.Assembler;
using Atomix.Assembler.x86;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.Kernel_H.arch.x86
{
    public static class PortIO
    {
        /// <summary>
        /// Read 8 bit from IO/Port
        /// </summary>
        /// <param name="aAddress">Address of memory</param>
        /// <returns></returns>
        [Assembly(0x0)]
        public static byte In8(UInt16 aAddress)
        {
            //Load address into EDX
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EDX, SourceReg = Registers.EBP, SourceDisplacement = 0x8, SourceIndirect = true });
            //Set EAX = 0x00000000
            Core.AssemblerCode.Add(new Xor { DestinationReg = Registers.EAX, SourceReg = Registers.EAX });
            //Read 8 byte And put result into EAX (AL)
            Core.AssemblerCode.Add(new In { DestinationReg = Registers.AL, SourceReg = Registers.DX });
            //Save value
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBP, SourceReg = Registers.EAX, DestinationDisplacement = 0x8, DestinationIndirect = true });

            return 0x0;
        }

        /// <summary>
        /// Write 8 bit from IO/Port
        /// </summary>
        /// <param name="aAddress">Address of memory</param>
        /// <returns></returns>
        [Assembly(0x8)]
        public static void Out8(UInt16 aAddress, byte aValue)
        {
            //Load address into EDX
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EDX, SourceReg = Registers.EBP, SourceDisplacement = 0xC, SourceIndirect = true });
            //Load value into EAX
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceDisplacement = 0x8, SourceIndirect = true });
            //Write 8 byte
            Core.AssemblerCode.Add(new Out { DestinationReg = Registers.DX, SourceReg = Registers.AL });
        }

        /// <summary>
        /// Read 16 bit from IO/Port
        /// </summary>
        /// <param name="aAddress">Address of memory</param>
        /// <returns></returns>
        [Assembly(0x0)]
        public static ushort In16(UInt16 aAddress)
        {
            //Load address into EDX
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EDX, SourceReg = Registers.EBP, SourceDisplacement = 0x8, SourceIndirect = true });
            //Set EAX = 0x00000000
            Core.AssemblerCode.Add(new Xor { DestinationReg = Registers.EAX, SourceReg = Registers.EAX });
            //Read 16 byte And put result into EAX (AX)
            Core.AssemblerCode.Add(new In { DestinationReg = Registers.AX, SourceReg = Registers.DX });
            //Save value
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBP, SourceReg = Registers.EAX, DestinationDisplacement = 0x8, DestinationIndirect = true });

            return 0x0;
        }

        /// <summary>
        /// Write 16 bit from IO/Port
        /// </summary>
        /// <param name="aAddress">Address of memory</param>
        /// <returns></returns>
        [Assembly(0x8)]
        public static void Out16(UInt16 aAddress, ushort aValue)
        {
            //Load address into EDX
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EDX, SourceReg = Registers.EBP, SourceDisplacement = 0xC, SourceIndirect = true });
            //Load value into EAX
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceDisplacement = 0x8, SourceIndirect = true });
            //Write 16 byte
            Core.AssemblerCode.Add(new Out { DestinationReg = Registers.DX, SourceReg = Registers.AX });
        }

        /// <summary>
        /// Read 32 bit from IO/Port
        /// </summary>
        /// <param name="aAddress">Address of memory</param>
        /// <returns></returns>
        [Assembly(0x0)]
        public static uint In32(UInt16 aAddress)
        {
            //Load address into EDX
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EDX, SourceReg = Registers.EBP, SourceDisplacement = 0x8, SourceIndirect = true });
            //Set EAX = 0x00000000
            Core.AssemblerCode.Add(new Xor { DestinationReg = Registers.EAX, SourceReg = Registers.EAX });
            //Read 16 byte And put result into EAX (AX)
            Core.AssemblerCode.Add(new In { DestinationReg = Registers.EAX, SourceReg = Registers.DX });
            //Save value
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBP, SourceReg = Registers.EAX, DestinationDisplacement = 0x8, DestinationIndirect = true });

            return 0x0;
        }

        /// <summary>
        /// Write 32 bit from IO/Port
        /// </summary>
        /// <param name="aAddress">Address of memory</param>
        /// <returns></returns>
        [Assembly(0x8)]
        public static void Out32(UInt16 aAddress, uint aValue)
        {
            //Load address into EDX
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EDX, SourceReg = Registers.EBP, SourceDisplacement = 0xC, SourceIndirect = true });
            //Load value into EAX
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceDisplacement = 0x8, SourceIndirect = true });
            //Write 16 byte
            Core.AssemblerCode.Add(new Out { DestinationReg = Registers.DX, SourceReg = Registers.EAX });
        }

        public static void Wait()
        {
            Out8(0x80, 0x22);
        }
    }
}
