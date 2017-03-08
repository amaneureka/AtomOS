/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Application Memory Access support functions
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;

namespace Atomixilc.Lib
{
    public static class Memory
    {
        /// <summary>
        /// Read 32 bit Memory at given address :)
        /// </summary>
        /// <param name="aAddress">Address of memory</param>
        /// <returns></returns>
        [NoException]
        [Assembly(false)]
        public static uint Read32(uint aAddress)
        {
            // Load address into EAX
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.ESP, SourceDisplacement = 0x4, SourceIndirect = true };
            // Read memory into EAX
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.EAX, SourceIndirect = true };
            // Return
            new Ret { Offset = 0x4 };

            return 0; // For c# error --> Don't make any sense for compiler
        }

        /// <summary>
        /// Read 16 bit Memory at given address :)
        /// </summary>
        /// <param name="aAddress">Address of memory</param>
        /// <returns></returns>
        [NoException]
        [Assembly(false)]
        public static ushort Read16(uint aAddress)
        {
            // Load address into EAX
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.ESP, SourceDisplacement = 0x4, SourceIndirect = true };
            // Read memory into EAX
            new Movzx { DestinationReg = Register.EAX, SourceReg = Register.EAX, SourceIndirect = true, Size = 16 };
            // Return
            new Ret { Offset = 0x4 };

            return 0; // For c# error --> Don't make any sense for compiler
        }

        /// <summary>
        /// Read 8 bit Memory at given address :)
        /// </summary>
        /// <param name="aAddress">Address of memory</param>
        /// <returns></returns>
        [NoException]
        [Assembly(false)]
        public static byte Read8(uint aAddress)
        {
            // Load address into EAX
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.ESP, SourceDisplacement = 0x4, SourceIndirect = true };
            // Read memory into EAX
            new Movzx { DestinationReg = Register.EAX, SourceReg = Register.EAX, SourceIndirect = true, Size = 8 };
            // Return
            new Ret { Offset = 0x4 };

            return 0; // For c# error --> Don't make any sense for compiler
        }

        /// <summary>
        /// Write 32 bit Memory at given address :)
        /// </summary>
        /// <param name="aAddress">Address of memory</param>
        /// <returns></returns>
        [NoException]
        [Assembly(false)]
        public static void Write32(uint aAddress, uint Value)
        {
            // Load address into EAX
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.ESP, SourceDisplacement = 0x8, SourceIndirect = true };
            // Load Value into EDX
            new Mov { DestinationReg = Register.EBX, SourceReg = Register.ESP, SourceDisplacement = 0x4, SourceIndirect = true };
            // Save value at mem Location
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.EBX, DestinationIndirect = true };
            // Return
            new Ret { Offset = 0x8 };
        }

        /// <summary>
        /// Write 16 bit Memory at given address :)
        /// </summary>
        /// <param name="aAddress">Address of memory</param>
        /// <returns></returns>
        [NoException]
        [Assembly(false)]
        public static void Write16(uint aAddress, ushort Value)
        {
            // Load address into EAX
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.ESP, SourceDisplacement = 0x8, SourceIndirect = true };
            // Load Value into EDX
            new Mov { DestinationReg = Register.EBX, SourceReg = Register.ESP, SourceDisplacement = 0x4, SourceIndirect = true };
            // Save value at mem Location
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.BX, DestinationIndirect = true, Size = 16 };
            // Return
            new Ret { Offset = 0x8 };
        }

        /// <summary>
        /// Write 8 bit Memory at given address :)
        /// </summary>
        /// <param name="aAddress">Address of memory</param>
        /// <returns></returns>
        [NoException]
        [Assembly(false)]
        public static void Write8(uint aAddress, byte Value)
        {
            // Load address into EAX
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.ESP, SourceDisplacement = 0x8, SourceIndirect = true };
            // Load Value into EDX
            new Mov { DestinationReg = Register.EBX, SourceReg = Register.ESP, SourceDisplacement = 0x4, SourceIndirect = true };
            // Save value at mem Location
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.BL, DestinationIndirect = true, Size = 8 };
            // Return
            new Ret { Offset = 0x8 };
        }

        [NoException]
        [Assembly(false)]
        public static void FastCopy(uint aDest, uint aSrc, uint aLen)
        {
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.ESP, SourceDisplacement = 0x4, SourceIndirect = true };
            new Mov { DestinationReg = Register.ESI, SourceReg = Register.ESP, SourceDisplacement = 0x8, SourceIndirect = true };
            new Mov { DestinationReg = Register.EDI, SourceReg = Register.ESP, SourceDisplacement = 0xC, SourceIndirect = true };

            new Mov { DestinationReg = Register.ECX, SourceReg = Register.EAX };
            new Shr { DestinationReg = Register.ECX, SourceRef = "0x2" };
            new Literal("rep movsd");
            new Mov { DestinationReg = Register.ECX, SourceReg = Register.EAX };
            new And { DestinationReg = Register.ECX, SourceRef = "0x3" };
            new Literal("rep movsb");

            // Return
            new Ret { Offset = 0xC };
        }

        [NoException]
        [Assembly(false)]
        public static unsafe void FastClear(uint Address, uint Length)
        {
            new Mov { DestinationReg = Register.EBX, SourceReg = Register.ESP, SourceDisplacement = 0x4, SourceIndirect = true };
            new Mov { DestinationReg = Register.EDI, SourceReg = Register.ESP, SourceDisplacement = 0x8, SourceIndirect = true };

            new Xor { DestinationReg = Register.EAX, SourceReg = Register.EAX };
            new Mov { DestinationReg = Register.ECX, SourceReg = Register.EBX };
            new Shr { DestinationReg = Register.ECX, SourceRef = "0x2" };
            new Literal("rep stosd");// Copy EAX to EDI
            new Mov { DestinationReg = Register.ECX, SourceReg = Register.EBX };
            new And { DestinationReg = Register.ECX, SourceRef = "0x3" };// Modulo by 4
            new Literal("rep stosb");

            // Return
            new Ret { Offset = 0x8 };
        }
    }
}
