using System;

using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;
using Atomix.Assembler;
using Atomix.Assembler.x86;
using asm = Atomix.Assembler.AssemblyHelper;

using libAtomixH.Core;
using libAtomixH.IO.Ports;
using libAtomixH.Threading;

namespace Kernel_H
{
    [Kernel (CPUArch.x86, "0x200000")]
    public static class Kernel
    {
        [Assembly, Plug ("Kernel_Main")]
        public static void main ()
        {
            // Initialize Multiboot
            asm.DataMember.Add (new AsmData ("MultibootSignature", BitConverter.GetBytes (0x1BADB002))); //0x200000
            asm.DataMember.Add (new AsmData ("MultibootFlags", BitConverter.GetBytes (65539))); //0x200004
            asm.DataMember.Add (new AsmData ("MultibootChecksum", BitConverter.GetBytes (-464433157))); //0x200008
            asm.DataMember.Add (new AsmData ("MultibootHeaderAddr", "dd MultibootSignature")); //0x20000C
            asm.DataMember.Add (new AsmData ("MultibootLoadAddr", "dd MultibootSignature")); //0x200010
            asm.DataMember.Add (new AsmData ("MultibootLoadEndAddr", "dd Compiler_End")); //0x200014
            asm.DataMember.Add (new AsmData ("MultibootBSSEndAddr", "dd Compiler_End")); //0x200018
            asm.DataMember.Add (new AsmData ("MultibootEntryAddr", "dd Kernel_Main")); //0x20001C
            asm.DataMember.Add (new AsmData ("Before_Kernel_Stack:", "TIMES 0x50000 db 0"));
            asm.DataMember.Add (new AsmData ("GDT_And_IDT_Content:", "TIMES 3000 db 0")); //0x250020 --> First IDT than GDT
            asm.DataMember.Add (new AsmData ("Stack_Entrypoint:", string.Empty));

            // Clear Interrupts
            asm.AssemblerCode.Add (new Cli ());

            // Initialize SSE
            asm.AssemblerCode.Add (new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.CR4 });
            asm.AssemblerCode.Add (new Or { DestinationReg = Registers.EAX, SourceRef = "0x100" });
            asm.AssemblerCode.Add (new Mov { DestinationReg = Registers.CR4, SourceReg = Registers.EAX });
            asm.AssemblerCode.Add (new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.CR4 });
            asm.AssemblerCode.Add (new Or { DestinationReg = Registers.EAX, SourceRef = "0x200" });
            asm.AssemblerCode.Add (new Mov { DestinationReg = Registers.CR4, SourceReg = Registers.EAX });
            asm.AssemblerCode.Add (new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.CR0 });
            asm.AssemblerCode.Add (new And { DestinationReg = Registers.EAX, SourceRef = "0xFFFFFFFD" });
            asm.AssemblerCode.Add (new Mov { DestinationReg = Registers.CR0, SourceReg = Registers.EAX });
            asm.AssemblerCode.Add (new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.CR0 });
            asm.AssemblerCode.Add (new And { DestinationReg = Registers.EAX, SourceRef = "0x1" });
            asm.AssemblerCode.Add (new Mov { DestinationReg = Registers.CR0, SourceReg = Registers.EAX });

            // Setup stack pointer
            asm.AssemblerCode.Add (new Mov { DestinationReg = Registers.ESP, SourceRef = "Stack_Entrypoint" });
            asm.AssemblerCode.Add (new Jmp { DestinationRef = "Kernel_Start" });
        }

        [Plug ("Kernel_Start")]
        public static void start ()
        {
            // Setup Multiboot
            Multiboot.Setup ();

            // Clear Interrupts
            Native.ClearInterrupt ();

            // Setup PIC
            PIC.Setup ();

            // Setup GDT and enter protected mode
            GDT.Setup ();

            // Enable Interrupts
            Native.SetInterrupt ();

            // Setup Multitasking
            Scheduler.CreateTask (0, true);
            Scheduler.Init ();

            // Start our main kernel
            try
            {
                Caller.Start ();
            }
            catch (Exception ex)
            {
                // Handle the exception
            }

            // Halt the CPU
            while (true)
            {
                Native.ClearInterrupt ();
                Native.Halt ();
            }
        }
    }
}
