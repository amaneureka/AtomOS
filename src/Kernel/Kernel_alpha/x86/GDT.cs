using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;
using Atomix.Assembler;
using Atomix.Assembler.x86;
using Kernel_alpha.x86;
using Kernel_alpha.x86.Intrinsic;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Kernel_alpha.x86
{
    public static class GDT
    {
        private static uint _gdtTable = 0x10582C; //0x200020 + 2048 (IDT Content) + 6 (IDT Pointer) + <Some space unused>
        private static uint _gdtEntries = 0x10582C + 6;
        
        public enum Offset : byte
        {
            LimitLow = 0x00,
            BaseLow = 0x02,
            BaseMiddle = 0x04,
            Access = 0x05,
            Granularity = 0x06,
            BaseHigh = 0x07,
            TotalSize = 0x08
        };
        
        public static void Setup()
        {
            Memory.Clear(_gdtTable, 6);
            Native.Write16(_gdtTable, ((byte)Offset.TotalSize * 5) - 1);
            Native.Write32(_gdtTable + 2, _gdtEntries);
            
            Set_GDT_Gate(0, 0, 0, 0, 0);                // Null segment
            Set_GDT_Gate(1, 0, 0xFFFFFFFF, 0x9A, 0xCF); // Code segment
            Set_GDT_Gate(2, 0, 0xFFFFFFFF, 0x92, 0xCF); // Data segment
            //Set_GDT_Gate(3, 0, 0xFFFFFFFF, 0xFA, 0xCF); // User mode code segment
            //Set_GDT_Gate(4, 0, 0xFFFFFFFF, 0xF2, 0xCF); // User mode data segment

            Native.Lgdt(_gdtTable);
                        
            FlushGDT();
        }

        private static void Set_GDT_Gate(uint index, uint address, uint limit, byte access, byte granularity)
        {
            uint entry = (uint)(_gdtEntries + (index * (byte)Offset.TotalSize));
            Native.Write16(entry + (byte)Offset.BaseLow, (ushort)(address & 0xFFFF));
            Native.Write8(entry + (byte)Offset.BaseMiddle, (byte)((address >> 16) & 0xFF));
            Native.Write8(entry + (byte)Offset.BaseHigh, (byte)((address >> 24) & 0xFF));
            Native.Write16(entry + (byte)Offset.LimitLow, (ushort)(limit & 0xFFFF));
            Native.Write8(entry + (byte)Offset.Granularity, (byte)(((byte)(limit >> 16) & 0x0F) | (granularity & 0xF0)));
            Native.Write8(entry + (byte)Offset.Access, access);
        }
        
        [Assembly(0x0)]
        private static void FlushGDT()
        {
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.AX, SourceRef = "0x10", Size = 16 });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.DS, SourceReg = Registers.AX, Size = 16 });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ES, SourceReg = Registers.AX, Size = 16 });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.FS, SourceReg = Registers.AX, Size = 16 });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.GS, SourceReg = Registers.AX, Size = 16 });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.SS, SourceReg = Registers.AX, Size = 16 });
            Core.AssemblerCode.Add(new Literal("jmp 8:Far_Jumper.End"));
            Core.AssemblerCode.Add(new Label("Far_Jumper.End"));
        }
    }
}
