/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          I/O Port In out Functions
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomixilc;
using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;

namespace Atomix.Kernel_H.Arch.x86
{
    internal static class PortIO
    {
        /// <summary>
        /// Read 8 bit from IO/Port
        /// </summary>
        /// <param name="aAddress">Address of memory</param>
        /// <returns></returns>
        [Assembly(true)]
        internal static byte In8(uint aAddress)
        {
            // Load address into EDX
            new Mov { DestinationReg = Register.EDX, SourceReg = Register.EBP, SourceDisplacement = 0x8, SourceIndirect = true };
            // Set EAX = 0x00000000
            new Xor { DestinationReg = Register.EAX, SourceReg = Register.EAX };
            // Read 8 byte And put result into EAX (AL)
            new In { DestinationReg = Register.AL, SourceReg = Register.DX };
            // push EAX
            new Push { DestinationReg = Register.EAX };

            return 0x0;
        }

        /// <summary>
        /// Write 8 bit from IO/Port
        /// </summary>
        /// <param name="aAddress">Address of memory</param>
        /// <returns></returns>
        [Assembly(true)]
        internal static void Out8(uint aAddress, byte aValue)
        {
            // Load address into EDX
            new Mov { DestinationReg = Register.EDX, SourceReg = Register.EBP, SourceDisplacement = 0xC, SourceIndirect = true };
            // Load value into EAX
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.EBP, SourceDisplacement = 0x8, SourceIndirect = true };
            // Write 8 byte
            new Out { DestinationReg = Register.DX, SourceReg = Register.AL };
        }

        /// <summary>
        /// Read 16 bit from IO/Port
        /// </summary>
        /// <param name="aAddress">Address of memory</param>
        /// <returns></returns>
        [Assembly(true)]
        internal static ushort In16(uint aAddress)
        {
            // Load address into EDX
            new Mov { DestinationReg = Register.EDX, SourceReg = Register.EBP, SourceDisplacement = 0x8, SourceIndirect = true };
            // Set EAX = 0x00000000
            new Xor { DestinationReg = Register.EAX, SourceReg = Register.EAX };
            // Read 16 byte And put result into EAX (AX)
            new In { DestinationReg = Register.AX, SourceReg = Register.DX };
            // push EAX
            new Push { DestinationReg = Register.EAX };

            return 0x0;
        }

        /// <summary>
        /// Write 16 bit from IO/Port
        /// </summary>
        /// <param name="aAddress">Address of memory</param>
        /// <returns></returns>
        [Assembly(true)]
        internal static void Out16(uint aAddress, ushort aValue)
        {
            // Load address into EDX
            new Mov { DestinationReg = Register.EDX, SourceReg = Register.EBP, SourceDisplacement = 0xC, SourceIndirect = true };
            // Load value into EAX
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.EBP, SourceDisplacement = 0x8, SourceIndirect = true };
            // Write 16 byte
            new Out { DestinationReg = Register.DX, SourceReg = Register.AX };
        }

        /// <summary>
        /// Read 32 bit from IO/Port
        /// </summary>
        /// <param name="aAddress">Address of memory</param>
        /// <returns></returns>
        [Assembly(true)]
        internal static uint In32(uint aAddress)
        {
            // Load address into EDX
            new Mov { DestinationReg = Register.EDX, SourceReg = Register.EBP, SourceDisplacement = 0x8, SourceIndirect = true };
            // Set EAX = 0x00000000
            new Xor { DestinationReg = Register.EAX, SourceReg = Register.EAX };
            // Read 16 byte And put result into EAX (AX)
            new In { DestinationReg = Register.EAX, SourceReg = Register.DX };
            // push EAX
            new Push { DestinationReg = Register.EAX };

            return 0x0;
        }

        /// <summary>
        /// Write 32 bit from IO/Port
        /// </summary>
        /// <param name="aAddress">Address of memory</param>
        /// <returns></returns>
        [Assembly(true)]
        internal static void Out32(uint aAddress, uint aValue)
        {
            // Load address into EDX
            new Mov { DestinationReg = Register.EDX, SourceReg = Register.EBP, SourceDisplacement = 0xC, SourceIndirect = true };
            // Load value into EAX
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.EBP, SourceDisplacement = 0x8, SourceIndirect = true };
            // Write 16 byte
            new Out { DestinationReg = Register.DX, SourceReg = Register.EAX };
        }

        internal static void Read16(uint aAddress, UInt16[] xData)
        {
            for (int i = 0; i < xData.Length; i++)
            {
                xData[i] = In16(aAddress);
            }
        }

        internal static void Read16(uint aAddress, byte[] xData)
        {
            for (int i = 0; i < xData.Length; i += 2)
            {
                var aData = In16(aAddress);
                xData[i] = (byte)(aData & 0xFF);
                xData[i + 1] = (byte)(aData >> 8);
            }
        }

        internal static void Read16(uint aAddress, byte[] xData, int size)
        {
            Read16(aAddress, xData);

            for (int i = xData.Length - 1; i < size; i += 2)
                In16(aAddress);
        }

        internal static void Write16(uint aAddress, UInt16[] xData)
        {
            for (int i = 0; i < xData.Length; i++)
            {
                Out16(aAddress, xData[i]);
            }
        }

        internal static void Write16(uint aAddress, byte[] xData)
        {
            for (int i = 0; i < xData.Length; i += 2)
            {
                Out16(aAddress, (ushort)(xData[i + 1] << 8 | xData[i]));
            }
        }

        internal static void Wait()
        {
            Out8(0x80, 0x22);
        }
    }
}
