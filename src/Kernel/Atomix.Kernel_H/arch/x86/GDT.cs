/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Kernel GDT Setup
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomix.Kernel_H.Core;
using Atomix.CompilerExt.Attributes;

using Atomix.Assembler;
using Atomix.Assembler.x86;
using Core = Atomix.Assembler.AssemblyHelper;

using System.Runtime.InteropServices;

namespace Atomix.Kernel_H.Arch.x86
{
    public static unsafe class GDT
    {
        private static uint gdt;
        private static GDT_Entries* gdt_entries;

        public static void Setup()
        {
            gdt = Heap.kmalloc(46);
            gdt_entries = (GDT_Entries*)(gdt + 6);

            Memory.Write16(gdt, (0x8 * 6) - 1);
            Memory.Write32(gdt + 2, (uint)gdt_entries);

            Debug.Write("GDT Setup!!\n");
            Debug.Write("       Base Address::%d\n", gdt);
            
            Set_GDT_Gate(0, 0, 0, 0, 0);                // Null segment
            Set_GDT_Gate(1, 0, 0xFFFFFFFF, 0x9A, 0xCF); // Code segment
            Set_GDT_Gate(2, 0, 0xFFFFFFFF, 0x92, 0xCF); // Data segment
            Set_GDT_Gate(3, 0, 0xFFFFFFFF, 0xFA, 0xCF); // User mode code segment
            Set_GDT_Gate(4, 0, 0xFFFFFFFF, 0xF2, 0xCF); // User mode data segment
            
            SetupGDT(gdt);
        }
        
        private static void Set_GDT_Gate(uint index, uint address, uint limit, byte access, byte granularity)
        {
            gdt_entries[index].BaseLow = (ushort)(address & 0xFFFF);
            gdt_entries[index].BaseMiddle = (byte)(address >> 16);
            gdt_entries[index].BaseHigh = (byte)(address >> 24);

            gdt_entries[index].LimitLow = (ushort)(limit & 0xFFFF);
            gdt_entries[index].Granularity = (byte)(limit >> 16);

            gdt_entries[index].Granularity |= (byte)(granularity & 0xF0);
            gdt_entries[index].Access = access;
            Debug.Write("       Gate-%d Present\n", index);
        }
        
        [Assembly(true)]
        private static void SetupGDT(uint gdtpointer)
        {
            AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 0x8 });
            AssemblyHelper.AssemblerCode.Add(new Literal ("lgdt [EAX]"));
            AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceRef = "0x10" });
            AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.DS, SourceReg = Registers.EAX, Size = 16 });
            AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.ES, SourceReg = Registers.EAX, Size = 16 });
            AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.FS, SourceReg = Registers.EAX, Size = 16 });
            AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.GS, SourceReg = Registers.EAX, Size = 16 });
            AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.SS, SourceReg = Registers.EAX, Size = 16 });
            AssemblyHelper.AssemblerCode.Add(new Literal ("jmp 0x8:Far_Jumper.End"));
            AssemblyHelper.AssemblerCode.Add(new Label ("Far_Jumper.End"));
        }

        [StructLayout(LayoutKind.Explicit, Size = 0x8)]
        struct GDT_Entries
        {
            [FieldOffset(0)]
            public ushort LimitLow;
            [FieldOffset(2)]
            public ushort BaseLow;
            [FieldOffset(4)]
            public byte BaseMiddle;
            [FieldOffset(5)]
            public byte Access;
            [FieldOffset(6)]
            public byte Granularity;
            [FieldOffset(7)]
            public byte BaseHigh;
        }
    }
}
