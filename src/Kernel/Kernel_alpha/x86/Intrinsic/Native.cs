using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;
using Atomix.Assembler;
using Atomix.Assembler.x86;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Kernel_alpha.x86.Intrinsic
{
    public static class Native
    {
        /// <summary>
        /// Clear Interrupt Flag
        /// </summary>
        [Assembly(0x0)]        
        public static void ClearInterrupt()
        {
            Core.AssemblerCode.Add(new Cli());
        }

        /// <summary>
        /// Setup Interrupt Flag
        /// </summary>
        [Assembly(0x0)]
        public static void SetInterrupt()
        {
            Core.AssemblerCode.Add(new Sti());
        }

        /// <summary>
        /// Halt CPU
        /// </summary>
        [Assembly(0x0)]
        public static void Halt()
        {
            Core.AssemblerCode.Add(new Literal("hlt"));
        }
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
        public static ushort In32(UInt16 aAddress)
        {
            //Load address into EDX
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EDX, SourceReg = Registers.EBP, SourceDisplacement = 0x8, SourceIndirect = true });
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
        /// <summary>
        /// Load GDT Address
        /// </summary>
        /// <param name="aAddress">Address of GDT Entries</param>
        [Assembly(0x4)]
        public static void Lgdt(uint aAddress)
        {
            //Load Address into EAX
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 0x8 });
            //Use Lgdt
            Core.AssemblerCode.Add(new Literal("lgdt [EAX]"));
        }

        /// <summary>
        /// Load IDT Address
        /// </summary>
        /// <param name="aAddress">Address of GDT Entries</param>
        [Assembly(0x4)]
        public static void Lidt(uint aAddress)
        {
            //Load Address into EAX
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 0x8 });
            //Use Lidt
            Core.AssemblerCode.Add(new Literal("lidt [EAX]"));
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

        public static void Wait()
        {
            Out8(0x80, 0x22);
        }
    }
}
