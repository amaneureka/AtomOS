/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Kernel IDT Setup
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomixilc.Lib;
using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;

using Atomix.Kernel_H.Core;

using System.Runtime.InteropServices;

namespace Atomix.Kernel_H.Arch.x86
{
    internal delegate void InterruptHandler(ref IRQContext state);

    [StructLayout(LayoutKind.Explicit, Size = 56)]
    internal struct IRQContext
    {
        [FieldOffset(0)]
        public int EDI;
        [FieldOffset(4)]
        public int ESI;
        [FieldOffset(8)]
        public int EBP;
        [FieldOffset(12)]
        public int ESP;
        [FieldOffset(16)]
        public int EBX;
        [FieldOffset(20)]
        public int EDX;
        [FieldOffset(24)]
        public int ECX;
        [FieldOffset(28)]
        public int EAX;
        [FieldOffset(32)]
        public int Interrupt;
        [FieldOffset(36)]
        public int ErrorCode;
        [FieldOffset(40)]
        public int EIP;
        [FieldOffset(44)]
        public int CS;
        [FieldOffset(48)]
        public int EFlags;
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

            LoadIDT(idt, idt_entries);
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
                Debug.Write("Unhandled Interrupt %d\n", (uint)interrupt);

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
        private static void LoadIDT(uint idt_table, uint idt_entries)
        {
            new Cli ();
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.EBP, SourceDisplacement = 0x8, SourceIndirect = true };
            for (int i = 0; i <= 0xFF; i++)
            {
                if (i == 1 || i == 3)
                    continue;

                var xHex = i.ToString("X2");

                new Mov { DestinationReg = Register.EBX, SourceRef = "__ISR_Handler_" + xHex };
                new Mov { DestinationReg = Register.EAX, DestinationIndirect = true, DestinationDisplacement = (i * 8) + 0, SourceReg = Register.BX, Size = 16 };

                new Mov { DestinationReg = Register.EAX, DestinationIndirect = true, DestinationDisplacement = (i * 8) + 2, SourceRef = "0x8", Size = 8 };
                new Mov { DestinationReg = Register.EAX, DestinationIndirect = true, DestinationDisplacement = (i * 8) + 5, SourceRef = "0x8E", Size = 8 };

                new Shr { DestinationReg = Register.EBX, SourceRef = "0x10" };

                new Mov { DestinationReg = Register.EAX, DestinationIndirect = true, DestinationDisplacement = (i * 8) + 6, SourceReg = Register.BX, Size = 16 };
            }
            var xLabel = Label.Primary + ".End";
            new Jmp { DestinationRef = xLabel };

            var xInterruptsWithParam = new int[] { 8, 10, 11, 12, 13, 14 };
            for (int i = 0; i <= 255; i++)
            {
                if (i == 1 || i == 3 || i == 0x20 || i == 0x75)
                    continue;

                var xHex = i.ToString("X2");
                new Label ("__ISR_Handler_" + xHex);

                new Cli ();
                if(Array.IndexOf(xInterruptsWithParam, i) == -1)
                {
                    new Push { DestinationRef = "0x0" };
                }

                new Push { DestinationRef = "0x" + xHex };
                new Pushad();
                new Mov { DestinationReg = Register.EAX, SourceReg = Register.ESP };
                new And { DestinationReg = Register.ESP, SourceRef = "0xFFFFFFF0" };
                new Sub { DestinationReg = Register.ESP, SourceRef = "0x200" };
                new Literal("fxsave [ESP]");
                new Push { DestinationReg = Register.EAX };
                new Push { DestinationReg = Register.EAX };
                new Literal("jmp 8:__ISR_Handler_" + xHex + ".SetCS");

                new Label("__ISR_Handler_" + xHex + ".SetCS");
                new Call { DestinationRef = "__Interrupt_Handler__", IsLabel = true };
                new Pop { DestinationReg = Register.EAX };
                new Literal("fxrstor [ESP]");
                new Mov { DestinationReg = Register.ESP, SourceReg = Register.EAX };
                new Popad();
                new Add { DestinationReg = Register.ESP, SourceRef = "0x8" };
                new Sti();
                new Iret();
            }

            new Label (xLabel);
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.EBP, SourceDisplacement = 0xC, SourceIndirect = true };
            new Literal ("lidt [EAX]");
        }
    }
}
