using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;
using Atomix.Assembler;
using Atomix.Assembler.x86;
using Kernel_alpha.x86;
using Kernel_alpha.Drivers;
using Kernel_alpha.x86.Intrinsic;
using Core = Atomix.Assembler.AssemblyHelper;
using System.Runtime.InteropServices;

namespace Kernel_alpha.x86
{
    public static class IDT
    {
        private static uint _idtTable = 0x250020;
        private static uint _idtEntries = 0x250020 + 6;
        
        private static IOPort idtTable;

        public enum Offset
        {
            BaseLow = 0x00,
            Select = 0x02,
            Always0 = 0x04,
            Flags = 0x05,
            BaseHigh = 0x06,
            TotalSize = 0x08
        };

        public static void Setup()
        {
            Memory.Clear(_idtTable, 6);

            Native.Write16(_idtTable, ((byte)Offset.TotalSize * 256) - 1);
            Native.Write32(_idtTable + 2, _idtEntries);

            /* For now hardcode IDT */
            UpdateIDT();

            /* Tell CPU About IDT Table location */
            Native.Lidt(_idtTable);
        }

        [Plug("__Interrupt_Handler__")]
        private static unsafe void ProcessInterrupt(ref IRQContext xContext)
        {
            var INT = xContext.Interrupt;

            if (INT < 0x13 && INT >= 0) // [0, 19) --> Exceptions
            {
                #region Handle
                const string xHex = "0123456789ABCDEF";
                unsafe
                {
                    byte* xAddress = (byte*)0xB8000;
                    xAddress[0] = (byte)' ';
                    xAddress[1] = 0x0C;
                    xAddress[2] = (byte)'*';
                    xAddress[3] = 0x0C;
                    xAddress[4] = (byte)'*';
                    xAddress[5] = 0x0C;
                    xAddress[6] = (byte)'*';
                    xAddress[7] = 0x0C;
                    xAddress[8] = (byte)' ';
                    xAddress[9] = 0x0C;
                    xAddress[10] = (byte)'C';
                    xAddress[11] = 0x0C;
                    xAddress[12] = (byte)'P';
                    xAddress[13] = 0x0C;
                    xAddress[14] = (byte)'U';
                    xAddress[15] = 0x0C;
                    xAddress[16] = (byte)' ';
                    xAddress[17] = 0x0C;
                    xAddress[18] = (byte)'E';
                    xAddress[19] = 0x0C;
                    xAddress[20] = (byte)'x';
                    xAddress[21] = 0x0C;
                    xAddress[22] = (byte)'c';
                    xAddress[23] = 0x0C;
                    xAddress[24] = (byte)'e';
                    xAddress[25] = 0x0C;
                    xAddress[26] = (byte)'p';
                    xAddress[27] = 0x0C;
                    xAddress[28] = (byte)'t';
                    xAddress[29] = 0x0C;
                    xAddress[30] = (byte)'i';
                    xAddress[31] = 0x0C;
                    xAddress[32] = (byte)'o';
                    xAddress[33] = 0x0C;
                    xAddress[34] = (byte)'n';
                    xAddress[35] = 0x0C;
                    xAddress[36] = (byte)' ';
                    xAddress[37] = 0x0C;
                    xAddress[38] = (byte)'x';
                    xAddress[39] = 0x0C;
                    xAddress[40] = (byte)xHex[(int)((INT >> 4) & 0xF)];
                    xAddress[41] = 0x0C;
                    xAddress[42] = (byte)xHex[(int)(INT & 0xF)];
                    xAddress[43] = 0x0C;
                    xAddress[44] = (byte)' ';
                    xAddress[45] = 0x0C;
                    xAddress[46] = (byte)'*';
                    xAddress[47] = 0x0C;
                    xAddress[48] = (byte)'*';
                    xAddress[49] = 0x0C;
                    xAddress[50] = (byte)'*';
                    xAddress[51] = 0x0C;
                    xAddress[52] = (byte)' ';
                    xAddress[53] = 0x0C;
                }
                #endregion
                while (true) ;
            }
            else if (INT >= 0x20 && INT < 0x30) //[32, 48) --> Hardware Interrupts
            {
                var xIRQ = (INT - 0x20);
                switch (xIRQ)
                {
                    case 1:
                        Global.KBD.HandleIRQ ();
                        break;
                    case 12:
                        Global.Mouse.HandleIRQ();
                        break;
                    case 7://Spurious IRQs
                        return;
                    case 14:
                        Global.PrimaryIDE.IRQInvoked = true;
                        break;
                    case 15:
                        Global.SecondayIDE.IRQInvoked = true;
                        break;
                }
                PIC.SendEndOfInterrupt((byte)xContext.Interrupt);
            }
        }

        [Assembly(0x0)]
        private static void UpdateIDT()
        {
            Core.AssemblerCode.Add(new Cli());
            for (int i = 0; i <= 255; i++)
            {
                if (i == 1 || i == 3)
                    continue;

                var xHex = i.ToString("X2");

                Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceRef = "__ISR_Handler_" + xHex });
                Core.AssemblerCode.Add(new Mov { DestinationRef = "GDT_And_IDT_Content", DestinationIndirect = true, DestinationDisplacement = (i * 8) + 0 + 6, SourceReg = Registers.AL, Size = 8 });
                Core.AssemblerCode.Add(new Mov { DestinationRef = "GDT_And_IDT_Content", DestinationIndirect = true, DestinationDisplacement = (i * 8) + 1 + 6, SourceReg = Registers.AH, Size = 8 });

                Core.AssemblerCode.Add(new Mov { DestinationRef = "GDT_And_IDT_Content", DestinationIndirect = true, DestinationDisplacement = (i * 8) + 2 + 6, SourceRef = "0x8", Size = 8 });
                Core.AssemblerCode.Add(new Mov { DestinationRef = "GDT_And_IDT_Content", DestinationIndirect = true, DestinationDisplacement = (i * 8) + 5 + 6, SourceRef = "0x8E", Size = 8 });
                Core.AssemblerCode.Add(new Shr { DestinationReg = Registers.EAX, SourceRef = "0x10" });
                Core.AssemblerCode.Add(new Mov { DestinationRef = "GDT_And_IDT_Content", DestinationIndirect = true, DestinationDisplacement = (i * 8) + 6 + 6, SourceReg = Registers.AL, Size = 8 });
                Core.AssemblerCode.Add(new Mov { DestinationRef = "GDT_And_IDT_Content", DestinationIndirect = true, DestinationDisplacement = (i * 8) + 7 + 6, SourceReg = Registers.AH, Size = 8 });
            }

            Core.AssemblerCode.Add(new Jmp { DestinationRef = "__Update_IDT__.END" });

            var xInterruptsWithParam = new int[] { 8, 10, 11, 12, 13, 14 };
            for (int i = 0; i <= 255; i++)
            {
                if (i == 0x20 || i == 1 || i == 3)
                    continue; //No IRQ0 here

                var xHex = i.ToString("X2");
                Core.AssemblerCode.Add(new Label("__ISR_Handler_" + xHex));
                if (Array.IndexOf(xInterruptsWithParam, i) == -1)
                {
                    Core.AssemblerCode.Add(new Push { DestinationRef = "0x0" });
                }
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
                Core.AssemblerCode.Add(new Iret());
            }

            Core.AssemblerCode.Add(new Label("__Update_IDT__.END"));
        }

        #region Defines
        [StructLayout(LayoutKind.Explicit, Size = 60)]
        public struct IRQContext
        {
            [FieldOffset(0)]
            public uint MMXContext_Pointer;
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
            public uint Param;
            [FieldOffset(44)]
            public uint EIP;
            [FieldOffset(48)]
            public uint CS;
            [FieldOffset(52)]
            public uint EFlags;
            [FieldOffset(56)]
            public uint UserESP;
        }

        #endregion
    }
}
