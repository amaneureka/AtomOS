/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Kernel GDT Setup
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomixilc.Lib;
using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;

using Atomix.Kernel_H.Core;

using System.Runtime.InteropServices;

namespace Atomix.Kernel_H.Arch.x86
{
    internal static unsafe class GDT
    {
        private static uint gdt;
        private static GDT_Entries* gdt_entries;

        internal static void Setup()
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

        private static void Set_GDT_Gate(int index, uint address, uint limit, byte access, byte granularity)
        {
            var gdt_entry = gdt_entries + index;
            gdt_entry->BaseLow = (ushort)address;
            gdt_entry->BaseMiddle = (byte)(address >> 16);
            gdt_entry->BaseHigh = (byte)(address >> 24);

            gdt_entry->LimitLow = (ushort)limit;
            gdt_entry->Granularity = (byte)(limit >> 16);

            gdt_entry->Granularity |= (byte)(granularity & 0xF0);
            gdt_entry->Access = access;
        }

        [Assembly(true)]
        private static void SetupGDT(uint gdtpointer)
        {
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.EBP, SourceIndirect = true, SourceDisplacement = 0x8 };
            new Literal ("lgdt [EAX]");
            new Mov { DestinationReg = Register.EAX, SourceRef = "0x10" };
            new Mov { DestinationReg = Register.DS, SourceReg = Register.EAX, Size = 16 };
            new Mov { DestinationReg = Register.ES, SourceReg = Register.EAX, Size = 16 };
            new Mov { DestinationReg = Register.FS, SourceReg = Register.EAX, Size = 16 };
            new Mov { DestinationReg = Register.GS, SourceReg = Register.EAX, Size = 16 };
            new Mov { DestinationReg = Register.SS, SourceReg = Register.EAX, Size = 16 };
            new Jmp { Selector = 0x8, DestinationRef = ".End" };
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
