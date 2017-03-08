/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Kernel Start point
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomixilc;
using Atomixilc.Lib;
using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;

using Atomix.Kernel_H.Core;
using Atomix.Kernel_H.Devices;
using Atomix.Kernel_H.Arch.x86;
using Atomix.Kernel_H.Drivers.Video;
using Atomix.Kernel_H.IO.FileSystem;

namespace Atomix.Kernel_H
{
    /// <summary>
    /// Startpoint for x86 CPU
    /// Kernel will be organised at 0xC0000000
    /// </summary>
    [Entrypoint(Architecture.x86, "(_Kernel_Main - 0xC0000000)")]
    public static class Startx86
    {
        [NoException]
        [Assembly(false)]
        public static void main()
        {
            const uint MultibootMagic = 0x1BADB002;
            const uint MultibootFlags = 0x10007;
            const uint InitalStackSize = 0x50000;
            const uint InitalHeapSize = 0x100000;

            /* Multiboot Config */
            new Literal("MultibootSignature dd {0}", MultibootMagic);
            new Literal("MultibootFlags dd {0}", MultibootFlags);
            new Literal("MultibootChecksum dd {0}", -(MultibootMagic + MultibootFlags));
            new Literal("MultibootHeaderAddr dd {0}", 0);
            new Literal("MultibootLoadAddr dd {0}", 0);
            new Literal("MultibootLoadEndAddr dd {0}", 0);
            new Literal("MultibootBSSEndAddr dd {0}", 0);
            new Literal("MultibootEntryAddr dd {0}", 0);
            new Literal("MultibootVesaMode dd {0}", 0);
            new Literal("MultibootVesaWidth dd {0}", 1024);
            new Literal("MultibootVesaHeight dd {0}", 768);
            new Literal("MultibootVesaDepth dd {0}", 32);

            Helper.InsertData("InitialStack", InitalStackSize);
            Helper.InsertData("InitialHeap", InitalHeapSize);

            new Label("_Kernel_Main");

            #region KernelPageDirectory
            Helper.InsertData(new AsmData("align 0x1000"));
            Helper.InsertData(new AsmData("KernelPageDirectory:"));
            Helper.InsertData(new AsmData("dd (KernelPageTable - 0xC0000000 + 0x3)"));
            Helper.InsertData(new AsmData("times (0x300 - 1) dd 0"));
            Helper.InsertData(new AsmData("dd (KernelPageTable - 0xC0000000 + 0x3)"));
            Helper.InsertData(new AsmData("times (1024 - 0x300 - 1) dd 0"));
            var xPageTable = new uint[1024];
            for (int i = 0; i < 1024; i++)
                xPageTable[i] = (uint)((i * 0x1000) | 0x3);
            Helper.InsertData(new AsmData("KernelPageTable", xPageTable));
            #endregion

            /* Load Page Directory Base Register. */
            new Mov { DestinationReg = Register.ECX, SourceRef = "(KernelPageDirectory - 0xC0000000)" };
            new Mov { DestinationReg = Register.CR3, SourceReg = Register.ECX };

            /* Set PG bit in CR0 to enable paging. */
            new Mov { DestinationReg = Register.ECX, SourceReg = Register.CR0 };
            new Or { DestinationReg = Register.ECX, SourceRef = "0x80000000" };
            new Mov { DestinationReg = Register.CR0, SourceReg = Register.ECX };

            /* Prepare for our quantum jump to Higher address */
            new Lea { DestinationReg = Register.ECX, SourceRef = "Higher_Half_Kernel", SourceIndirect = true };
            new Jmp { DestinationRef = "ECX" };

            new Label("Higher_Half_Kernel");
            new Mov { DestinationRef = "KernelPageDirectory", DestinationIndirect = true, SourceRef = "0x0" };
            new Literal("invlpg [0]");

            /* Setup Kernel stack */
            new Mov { DestinationReg = Register.ESP, SourceRef = "InitialStack" };
            new Add { DestinationReg = Register.ESP, SourceRef = InitalStackSize.ToString() };

            /* Push relevent data to the stack */
            new Push { DestinationReg = Register.EAX };//Push Magic Number
            new Push { DestinationReg = Register.EBX };//Push Multiboot Address
            new Push { DestinationRef = "KernelPageDirectory" };
            new Push { DestinationRef = "InitialHeap" };

            /* Enable Floating Point Unit */
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.CR0 };
            new And { DestinationReg = Register.AX, SourceRef = "0xFFFD", Size = 16 };
            new Or { DestinationReg = Register.AX, SourceRef = "0x10", Size = 16 };
            new Mov { DestinationReg = Register.CR0, SourceReg = Register.EAX };
            new Literal("fninit");

            /* Enable SSE */
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.CR0 };
            new And { DestinationReg = Register.AX, SourceRef = "0xFFFB", Size = 16 };
            new Or { DestinationReg = Register.AX, SourceRef = "0x2", Size = 16 };
            new Mov { DestinationReg = Register.CR0, SourceReg = Register.EAX };
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.CR4 };
            new Or { DestinationReg = Register.AX, SourceRef = "0x600", Size = 16 };
            new Mov { DestinationReg = Register.CR4, SourceReg = Register.EAX };

            /* Call Kernel Start */
            new Cli();
            new Call { DestinationRef = "Kernel_Start", IsLabel = true };
        }

        /// <summary>
        /// Kernel Call point
        /// </summary>
        /// <param name="magic">Magic Number of Multiboot</param>
        /// <param name="address">Multiboot Address</param>
        [Label("Kernel_Start")]
        internal static void Start(uint magic, uint address, uint KernelDirectory, uint InitialHeap)
        {
            /* Kernel Logger init */
            Debug.Init();

            /* Initalize Heap */
            Heap.Init(InitialHeap);

            /* Multiboot Info Parsing */
            Multiboot.Setup(magic, address);

            /* Setup Paging */
            Paging.Setup(KernelDirectory);

            /* Setup GDT */
            GDT.Setup();

            /* Remap PIC */
            PIC.Setup();

            /* Setup IDT */
            IDT.Setup();

            /* Enable Interrupt */
            Native.Sti();

            /* Setup Scheduler */
            Scheduler.Init(KernelDirectory);

            /* Setup System Timer */
            Timer.Setup();

            /* Install SHM */
            SHM.Install();

            /* Initialise VBE 2.0 Driver */
            VBE.Init();

            /* Initialise Virtual File system */
            VirtualFileSystem.Setup();

            /* Setup Syscall */
            Syscall.Setup();

            /* Initialise C library */
            Libc.Init();

            try
            {
                Boot.Init();
            }
            catch (Exception e)
            {
                Debug.Write("[@SystemThread] => [EXIT]: %s\n", e.Message);
            }

            while (true);
        }
    }
}
