/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Kernel Start point
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;

using Atomix.Assembler;
using Atomix.Assembler.x86;
using Core = Atomix.Assembler.AssemblyHelper;

using Atomix.Kernel_H.core;
using Atomix.Kernel_H.devices;
using Atomix.Kernel_H.arch.x86;
using Atomix.Kernel_H.drivers.video;
using Atomix.Kernel_H.io.FileSystem;


namespace Atomix.Kernel_H
{
    /// <summary>
    /// Startpoint for x86 CPU
    /// Kernel will be organised at 0x10000
    /// </summary>
    [Kernel(CPUArch.x86, "0xC0000000")]
    public static class Startx86
    {
        [Assembly]
        public static void main()
        {
            const uint MultibootMagic = 0x1BADB002;
            const uint MultibootFlags = 0x10007;
            const uint InitalStackSize = 0x50000;
            const uint InitalHeapSize = 0x100000;

            /* Multiboot Config */
            Core.AssemblerCode.Add(new Literal("MultibootSignature dd {0}", MultibootMagic));
            Core.AssemblerCode.Add(new Literal("MultibootFlags dd {0}", 65543));
            Core.AssemblerCode.Add(new Literal("MultibootChecksum dd {0}", -(MultibootMagic + MultibootFlags)));
            Core.AssemblerCode.Add(new Literal("MultibootHeaderAddr dd {0}", 0));
            Core.AssemblerCode.Add(new Literal("MultibootLoadAddr dd {0}", 0));
            Core.AssemblerCode.Add(new Literal("MultibootLoadEndAddr dd {0}", 0));
            Core.AssemblerCode.Add(new Literal("MultibootBSSEndAddr dd {0}", 0));
            Core.AssemblerCode.Add(new Literal("MultibootEntryAddr dd {0}", 0));
            Core.AssemblerCode.Add(new Literal("MultibootVesaMode dd {0}", 0));
            Core.AssemblerCode.Add(new Literal("MultibootVesaWidth dd {0}", 1024));
            Core.AssemblerCode.Add(new Literal("MultibootVesaHeight dd {0}", 768));
            Core.AssemblerCode.Add(new Literal("MultibootVesaDepth dd {0}", 32));

            Core.InsertData(new AsmData("InitialStack", InitalStackSize));
            Core.InsertData(new AsmData("InitialHeap", InitalHeapSize));

            Core.AssemblerCode.Add(new Label("_Kernel_Main"));

            #region KernelPageDirectory
            Core.InsertData(new AsmData("align 0x1000", string.Empty));
            Core.InsertData(new AsmData("KernelPageDirectory:", "\ndd (KernelPageTable - 0xC0000000 + 0x3)\ntimes (0x300 - 1) dd 0\ndd (KernelPageTable - 0xC0000000 + 0x3)\ntimes (1024 - 0x300 - 1) dd 0"));
            var xPageTable = new string[1024];
            for (int i = 0; i < 1024; i++)
                xPageTable[i] = ((i * 0x1000) | 0x3).ToString();
            Core.InsertData(new AsmData("KernelPageTable", xPageTable));
            #endregion

            /* Load Page Directory Base Register. */
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, SourceRef = "(KernelPageDirectory - 0xC0000000)" });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.CR3, SourceReg = Registers.ECX });

            /* Set PG bit in CR0 to enable paging. */
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, SourceReg = Registers.CR0 });
            Core.AssemblerCode.Add(new Or { DestinationReg = Registers.ECX, SourceRef = "0x80000000" });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.CR0, SourceReg = Registers.ECX });

            /* Prepare for our quantum jump to Higher address */
            Core.AssemblerCode.Add(new Lea { DestinationReg = Registers.ECX, SourceRef = "Higher_Half_Kernel", SourceIndirect = true });
            Core.AssemblerCode.Add(new Jmp { DestinationRef = "ECX" });

            Core.AssemblerCode.Add(new Label("Higher_Half_Kernel"));
            Core.AssemblerCode.Add(new Mov { DestinationRef = "KernelPageDirectory", DestinationIndirect = true, SourceRef = "0x0" });
            Core.AssemblerCode.Add(new Literal("invlpg [0]"));

            /* Setup Kernel stack */
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ESP, SourceRef = "InitialStack" });
            Core.AssemblerCode.Add(new Add { DestinationReg = Registers.ESP, SourceRef = InitalStackSize.ToString() });

            /* Push relevent data to the stack */
            Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EAX });//Push Magic Number
            Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EBX });//Push Multiboot Address
            Core.AssemblerCode.Add(new Push { DestinationRef = "KernelPageDirectory" });
            Core.AssemblerCode.Add(new Push { DestinationRef = "InitialHeap" });

            /* Enable Floating Point Unit */
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.CR0 });
            Core.AssemblerCode.Add(new And { DestinationReg = Registers.AX, SourceRef = "0xFFFD", Size = 16 });
            Core.AssemblerCode.Add(new Or { DestinationReg = Registers.AX, SourceRef = "0x10", Size = 16 });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.CR0, SourceReg = Registers.EAX });
            Core.AssemblerCode.Add(new Literal("fninit"));

            /* Enable SSE */
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.CR0 });
            Core.AssemblerCode.Add(new And { DestinationReg = Registers.AX, SourceRef = "0xFFFB", Size = 16 });
            Core.AssemblerCode.Add(new Or { DestinationReg = Registers.AX, SourceRef = "0x2", Size = 16 });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.CR0, SourceReg = Registers.EAX });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.CR4 });
            Core.AssemblerCode.Add(new Or { DestinationReg = Registers.EAX, SourceRef = "0x600" });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.CR4, SourceReg = Registers.EAX });

            /* Call Kernel Start */
            Core.AssemblerCode.Add(new Cli());
            Core.AssemblerCode.Add(new Call("Kernel_Start", true));
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

            /* Setup Monitor */
            Monitor.Setup();

            /* Setup Paging */
            Paging.Setup(KernelDirectory);

            /* Setup GDT Again */
            GDT.Setup();

            /* Remap PIC */
            PIC.Setup();

            /* Setup IDT */
            IDT.Setup();

            /* Enable Interrupt */
            Native.Sti();

            /* Setup Scheduler */
            Scheduler.Init();

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

            /*
             * Scheduler must be called before Timer because,
             * just after calling timer, it will enable IRQ0 resulting in refrence call for switch task
             * Hence results in pagefault.
             */
            Scheduler.SystemProcess = new Process("System", KernelDirectory - 0xC0000000);

            /* System Thread */
            new Thread(Scheduler.SystemProcess, 0, 0, 10000).Start();

            try
            {
                Boot.Init();
            }
            catch (Exception e)
            {
                Debug.Write("[@SystemThread] => [EXIT]: %s\n", e.Message);
            }

            while(true);
        }
    }
}
