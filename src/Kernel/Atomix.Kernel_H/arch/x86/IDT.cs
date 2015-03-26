/* Copyright (C) Atomix Development, Inc - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Aman Priyadarshi <aman.eureka@gmail.com>, December 2014
 * 
 * IDT.cs
 *      Interrupt Descript Table Setup and Interrupt Handler
 *      
 *      History:
 *          19-12-14    File Created    Aman Priyadarshi
 */

using System;
using System.Collections.Generic;

using Atomix.Kernel_H.core;

using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;

using Atomix.Assembler;
using Atomix.Assembler.x86;
using Core = Atomix.Assembler.AssemblyHelper;

using System.Runtime.InteropServices;

namespace Atomix.Kernel_H.arch.x86
{
    public static class IDT
    {
        private static uint idt;
        private static uint idt_entries;
        private static InterruptHandler[] xINT;

        public delegate void InterruptHandler(ref IRQContext state);

        public static void Setup()
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

        [Plug("__Interrupt_Handler__")]
        private static unsafe void ProcessInterrupt(ref IRQContext xContext)
        {
            var interrupt = xContext.Interrupt;
            var Handler = xINT[interrupt];

            if (Handler != null)
                Handler(ref xContext);
            else
                Fault.Handle(ref xContext);

            //Send End of Interrupt for IRQs
            if (interrupt >= 0x20)
                PIC.EndOfInterrupt(interrupt);
        }

        public static void RegisterInterrupt(InterruptHandler xHandler, uint Interrupt)
        {
            xINT[Interrupt] = xHandler;
            Debug.Write("Interrupt Handler Registered: %d\n", Interrupt);
        }

        [Assembly(0x4)]
        private static void LoadIDT(uint idt_table, uint idt_entries)
        {
            Core.AssemblerCode.Add(new Cli());
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceDisplacement = 0x8, SourceIndirect = true });
            for (int i = 0; i <= 0xFF; i++)
            {
                if (i == 1 || i == 3)
                    continue;

                var xHex = i.ToString("X2");
                Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBX, SourceRef = "__ISR_Handler_" + xHex });
                Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, DestinationIndirect = true, DestinationDisplacement = (i * 8) + 0, SourceReg = Registers.BL, Size = 8 });
                Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, DestinationIndirect = true, DestinationDisplacement = (i * 8) + 1, SourceReg = Registers.BH, Size = 8 });

                Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, DestinationIndirect = true, DestinationDisplacement = (i * 8) + 2, SourceRef = "0x8", Size = 8 });
                Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, DestinationIndirect = true, DestinationDisplacement = (i * 8) + 5, SourceRef = "0x8E", Size = 8 });
                Core.AssemblerCode.Add(new Shr { DestinationReg = Registers.EBX, SourceRef = "0x10" });
                Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, DestinationIndirect = true, DestinationDisplacement = (i * 8) + 6, SourceReg = Registers.BL, Size = 8 });
                Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, DestinationIndirect = true, DestinationDisplacement = (i * 8) + 7, SourceReg = Registers.BH, Size = 8 });
            }
            var xLabel = Label.PrimaryLabel + ".End";
            Core.AssemblerCode.Add(new Jmp { DestinationRef = xLabel });

            var xInterruptsWithParam = new int[] { 8, 10, 11, 12, 13, 14 };
            for (int i = 0; i <= 255; i++)
            {
                if (i == 1 || i == 3 || i == 0x20) //Timer Interrupt handled somewhere else
                    continue;

                var xHex = i.ToString("X2");
                Core.AssemblerCode.Add(new Label("__ISR_Handler_" + xHex));

                Core.AssemblerCode.Add(new Cli());
                if (Array.IndexOf(xInterruptsWithParam, i) == -1)
                    Core.AssemblerCode.Add(new Push { DestinationRef = "0x0" });
                Core.AssemblerCode.Add(new Push { DestinationRef = "0x" + xHex });
                Core.AssemblerCode.Add(new Pushad());
                Core.AssemblerCode.Add(new Sub { DestinationReg = Registers.ESP, SourceRef = "0x4" });
                Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.ESP });
                Core.AssemblerCode.Add(new And { DestinationReg = Registers.ESP, SourceRef = "0xFFFFFFF0" });
                Core.AssemblerCode.Add(new Sub { DestinationReg = Registers.ESP, SourceRef = "0x200" });
                Core.AssemblerCode.Add(new Literal("fxsave [ESP]"));
                Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.ESP, DestinationIndirect = true });
                Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EAX });
                Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EAX });
                Core.AssemblerCode.Add(new Literal("jmp 8:__ISR_Handler_" + xHex + ".SetCS"));
                
                Core.AssemblerCode.Add(new Label("__ISR_Handler_" + xHex + ".SetCS"));
                Core.AssemblerCode.Add(new Call("__Interrupt_Handler__"));
                Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                Core.AssemblerCode.Add(new Literal("fxrstor [ESP]"));
                Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ESP, SourceReg = Registers.EAX });
                Core.AssemblerCode.Add(new Add { DestinationReg = Registers.ESP, SourceRef = "0x4" });
                Core.AssemblerCode.Add(new Popad());
                Core.AssemblerCode.Add(new Add { DestinationReg = Registers.ESP, SourceRef = "0x8" });
                Core.AssemblerCode.Add(new Sti());
                Core.AssemblerCode.Add(new Iret());
            }

            Core.AssemblerCode.Add(new Label(xLabel));
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceDisplacement = 0xC, SourceIndirect = true });
            Core.AssemblerCode.Add(new Literal("lidt [EAX]"));
        }
    }
    #region Defines
    [StructLayout(LayoutKind.Explicit, Size = 56)]
    public struct IRQContext
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
    }

    #endregion
}
