/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Kernel IDT Setup
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomix.Kernel_H.Core;

using Atomix.CompilerExt.Attributes;

using Atomix.Assembler;
using Atomix.Assembler.x86;

using System.Runtime.InteropServices;

namespace Atomix.Kernel_H.Arch.x86
{
    internal delegate void InterruptHandler(ref IRQContext state);

    [StructLayout(LayoutKind.Explicit, Size = 56)]
    internal struct IRQContext
    {
        [FieldOffset(0)]
        public uint MMX_Context;
        [FieldOffset(4)]
        public uint EDI;
        [FieldOffset(8)]
        public uint ESI;
        [FieldOffset(12)]
        public uint EBP;
        [FieldOffset(16)]
        public uint ESP;
        [FieldOffset(20)]
        public uint EBX;
        [FieldOffset(24)]
        public uint EDX;
        [FieldOffset(28)]
        public uint ECX;
        [FieldOffset(32)]
        public uint EAX;
        [FieldOffset(36)]
        public uint Interrupt;
        [FieldOffset(40)]
        public uint ErrorCode;
        [FieldOffset(44)]
        public uint EIP;
        [FieldOffset(48)]
        public uint CS;
        [FieldOffset(52)]
        public uint EFlags;
    };

    internal static class IDT
    {
        private static uint idt;
        private static uint idt_entries;
        private static InterruptHandler[] xINT;

        internal static void Setup()
        {
            idt = Heap.kmalloc(2048 + 6);
            idt_entries = idt + 6;

            Memory.Write16(idt, ((byte)0x8 * 256) - 1);
            Memory.Write32(idt + 2, idt_entries);

            Debug.Write("IDT Setup!!\n");
            Debug.Write("       Table Address::%d\n", idt);
            Debug.Write("       Entry Address::%d\n", idt_entries);

            LoadIDT(idt);
            Debug.Write("       IDT-Loaded\n");

            xINT = new InterruptHandler[256];
        }

        [Label("__Interrupt_Handler__")]
        private static unsafe void ProcessInterrupt(ref IRQContext xContext)
        {
            var interrupt = xContext.Interrupt;
            var Handler = xINT[interrupt];

            if (Handler != null)
                Handler(ref xContext);
            else if (interrupt < 0x20)
                Fault.Handle(ref xContext);
            else
                Debug.Write("Unhandled Interrupt %d\n", interrupt);

            // Send End of Interrupt for IRQs
            if (interrupt >= 0x20)
                PIC.EndOfInterrupt(interrupt);
        }

        internal static void RegisterInterrupt(InterruptHandler xHandler, uint Interrupt)
        {
            xINT[Interrupt] = xHandler;
            Debug.Write("Interrupt Handler Registered: %d\n", Interrupt);
        }

        [Assembly(true)]
        private static void LoadIDT(uint idt_table)
        {
            AssemblyHelper.AssemblerCode.Add(new Cli ());
            AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceDisplacement = 0x8, SourceIndirect = true });
            for (int i = 0; i <= 0xFF; i++)
            {
                if (i == 1 || i == 3)
                    continue;

                var xHex = i.ToString("X2");

                AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBX, SourceRef = "__ISR_Handler_" + xHex });
                AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, DestinationIndirect = true, DestinationDisplacement = (i * 8) + 0, SourceReg = Registers.BX, Size = 16 });

                AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, DestinationIndirect = true, DestinationDisplacement = (i * 8) + 2, SourceRef = "0x8", Size = 8 });
                AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, DestinationIndirect = true, DestinationDisplacement = (i * 8) + 5, SourceRef = "0x8E", Size = 8 });

                AssemblyHelper.AssemblerCode.Add(new ShiftRight { DestinationReg = Registers.EBX, SourceRef = "0x10" });

                AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, DestinationIndirect = true, DestinationDisplacement = (i * 8) + 6, SourceReg = Registers.BX, Size = 16 });
            }
            var xLabel = Label.PrimaryLabel + ".End";
            AssemblyHelper.AssemblerCode.Add(new Jmp { DestinationRef = xLabel });

            var xInterruptsWithParam = new int[] { 8, 10, 11, 12, 13, 14 };
            for (int i = 0; i <= 255; i++)
            {
                if (i == 1 || i == 3 || i == 0x20) // Timer Interrupt handled somewhere else
                    continue;

                var xHex = i.ToString("X2");
                AssemblyHelper.AssemblerCode.Add(new Label ("__ISR_Handler_" + xHex));

                AssemblyHelper.AssemblerCode.Add(new Cli ());
                if(Array.IndexOf(xInterruptsWithParam, i) == -1)
                {
                    AssemblyHelper.AssemblerCode.Add(new Push { DestinationRef = "0x0" });
                }

                AssemblyHelper.AssemblerCode.Add(new Push { DestinationRef = "0x" + xHex });
                AssemblyHelper.AssemblerCode.Add(new Pushad());
                AssemblyHelper.AssemblerCode.Add(new Sub { DestinationReg = Registers.ESP, SourceRef = "0x4" });
                AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.ESP });
                AssemblyHelper.AssemblerCode.Add(new And { DestinationReg = Registers.ESP, SourceRef = "0xFFFFFFF0" });
                AssemblyHelper.AssemblerCode.Add(new Sub { DestinationReg = Registers.ESP, SourceRef = "0x200" });
                AssemblyHelper.AssemblerCode.Add(new Literal("fxsave [ESP]"));
                AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.ESP, DestinationIndirect = true });
                AssemblyHelper.AssemblerCode.Add(new Push { DestinationReg = Registers.EAX });
                AssemblyHelper.AssemblerCode.Add(new Push { DestinationReg = Registers.EAX });
                AssemblyHelper.AssemblerCode.Add(new Literal("jmp 8:__ISR_Handler_" + xHex + ".SetCS"));

                AssemblyHelper.AssemblerCode.Add(new Label("__ISR_Handler_" + xHex + ".SetCS"));
                AssemblyHelper.AssemblerCode.Add(new Call("__Interrupt_Handler__", true));
                AssemblyHelper.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                AssemblyHelper.AssemblerCode.Add(new Literal("fxrstor [ESP]"));
                AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.ESP, SourceReg = Registers.EAX });
                AssemblyHelper.AssemblerCode.Add(new Add { DestinationReg = Registers.ESP, SourceRef = "0x4" });
                AssemblyHelper.AssemblerCode.Add(new Popad());
                AssemblyHelper.AssemblerCode.Add(new Add { DestinationReg = Registers.ESP, SourceRef = "0x8" });
                AssemblyHelper.AssemblerCode.Add(new Sti());
                AssemblyHelper.AssemblerCode.Add(new Iret());
            }

            AssemblyHelper.AssemblerCode.Add(new Label (xLabel));
            AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceDisplacement = 0xC, SourceIndirect = true });
            AssemblyHelper.AssemblerCode.Add(new Literal ("lidt [EAX]"));
        }
    }
}
