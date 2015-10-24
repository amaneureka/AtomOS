/* Copyright (C) Atomix Development, Inc - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Aman Priyadarshi <aman.eureka@gmail.com>, December 2014
 * 
 * Start.cs
 *      Kernel Start point.
 *      
 *      History:
 *          19-12-14    File Created    Aman Priyadarshi
 */

using System;

using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;

using Atomix.Assembler;
using Atomix.Assembler.x86;
using Core = Atomix.Assembler.AssemblyHelper;

using Atomix.Kernel_H.gui;
using Atomix.Kernel_H.core;
using Atomix.Kernel_H.devices;
using Atomix.Kernel_H.arch.x86;
using Atomix.Kernel_H.drivers.input;
using Atomix.Kernel_H.drivers.video;
using Atomix.Kernel_H.io.FileSystem;

namespace Atomix.Kernel_H
{
    /// <summary>
    /// Startpoint for x86 CPU
    /// Kernel will be organised at 0x10000
    /// </summary>
    [Kernel(CPUArch.x86, "0xC0100000")]
    public static class Startx86
    {
        [Assembly, Plug("Kernel_Main")]
        public static void main()
        {
            Core.DataMember.Add(new AsmData("[map all main.map]", ""));

            /* Multiboot Header */
            Core.DataMember.Add(new AsmData("MultibootSignature", BitConverter.GetBytes(0x1BADB002)));
            Core.DataMember.Add(new AsmData("MultibootFlags", BitConverter.GetBytes(65543)));
            Core.DataMember.Add(new AsmData("MultibootChecksum", BitConverter.GetBytes(-464433161)));
            Core.DataMember.Add(new AsmData("MultibootHeaderAddr", "dd (MultibootSignature - 0xC0000000)"));
            Core.DataMember.Add(new AsmData("MultibootLoadAddr", "dd (MultibootSignature - 0xC0000000)"));
            Core.DataMember.Add(new AsmData("MultibootLoadEndAddr", "dd (Compiler_End - 0xC0000000)"));
            Core.DataMember.Add(new AsmData("MultibootBSSEndAddr", "dd (Compiler_End - 0xC0000000)"));
            Core.DataMember.Add(new AsmData("MultibootEntryAddr", "dd (Kernel_Main - 0xC0000000)"));
            Core.DataMember.Add(new AsmData("MultibootVesaMode", BitConverter.GetBytes(0)));
            Core.DataMember.Add(new AsmData("MultibootVesaWidth", BitConverter.GetBytes(1024)));
            Core.DataMember.Add(new AsmData("MultibootVesaHeight", BitConverter.GetBytes(768)));
            Core.DataMember.Add(new AsmData("MultibootVesaDepth", BitConverter.GetBytes(32)));
            Core.DataMember.Add(new AsmData("BeforeInitialStack:", "TIMES 327680 db 0"));
            Core.DataMember.Add(new AsmData("InitialStack:", string.Empty));
            Core.DataMember.Add(new AsmData("InitialHeap:", "TIMES 0x100000 db 0"));

            #region KernelPageDirectory
            Core.DataMember.Add(new AsmData("align 0x1000", string.Empty));
            Core.DataMember.Add(new AsmData("KernelPageDirectory:", "\ndd (KernelPageTable - 0xC0000000 + 0x3)\ntimes (0x300 - 1) dd 0\ndd (KernelPageTable - 0xC0000000 + 0x3)\ntimes (1024 - 0x300 - 1) dd 0"));
            var xPageTable = new string[1024];
            for (int i = 0; i < 1024; i++)
                xPageTable[i] = ((i * 0x1000) | 0x3).ToString();
            Core.DataMember.Add(new AsmData("KernelPageTable", xPageTable));
            #endregion
            #region Paging
            //Load Page Directory Base Register.
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, SourceRef = "(KernelPageDirectory - 0xC0000000)" });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.CR3, SourceReg = Registers.ECX });

            //Set PG bit in CR0 to enable paging.
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, SourceReg = Registers.CR0 });
            Core.AssemblerCode.Add(new Or { DestinationReg = Registers.ECX, SourceRef = "0x80000000" });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.CR0, SourceReg = Registers.ECX });
            #endregion

            //Prepare for our quantum jump to Higher address
            Core.AssemblerCode.Add(new Literal("lea ecx, [Higher_Half_Kernel]"));
            Core.AssemblerCode.Add(new Jmp { DestinationRef = "ECX" });

            Core.AssemblerCode.Add(new Label("Higher_Half_Kernel"));
            Core.AssemblerCode.Add(new Mov { DestinationRef = "KernelPageDirectory", DestinationIndirect = true, SourceRef = "0x0" });
            Core.AssemblerCode.Add(new Literal("invlpg [0]"));

            /* Setup Kernel stack */
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ESP, SourceRef = "InitialStack" });
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
            Core.AssemblerCode.Add(new Call("Kernel_Start"));
        }

        /// <summary>
        /// Kernel Call point
        /// </summary>
        /// <param name="magic">Magic Number of Multiboot</param>
        /// <param name="address">Multiboot Address</param>
        [Plug("Kernel_Start")]
        public static void Start(uint magic, uint address, uint KernelDirectory, uint InitialHeap)
        {
            /* Kernel Logger init */
            Debug.Init();

            /* Initalize Heap */
            Heap.Init(InitialHeap);

            /* Multiboot Info Parsing */
            Multiboot.Setup(magic, address);

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

            /* Initialise VBE 2.0 Driver */
            VBE.Init();

            /* Initialise Virtual File system */
            VirtualFileSystem.Setup();

            /* Setup PS/2 Keyboard */
            Keyboard.Setup();

            /* Setup PS/2 Mouse */
            Mouse.Setup();

            /*
             * Scheduler must be called before Timer because, 
             * just after calling timer, it will enable IRQ0 resulting in refrence call for switch task
             * Hence results in pagefault.
             */
            var System = new Process("System", KernelDirectory - 0xC0000000);

            /* System Thread */
            new Thread(System, 0, 0, 10000).Start();

            Compositor.Setup(System);
            /*
            var ATA = new drivers.buses.ATA.IDE(true);
            var parts = new MBR(ATA);
            if (!VirtualFileSystem.Mount("dev0", new FatFileSystem(parts.PartInfo[0])))
                Debug.Write("Mound Failed!\n");

            var data = VirtualFileSystem.GetFile("dev0\\AP");
            if (data == null)
                Debug.Write("File Not Found\n");
            else
            {
                var xData = new byte[10];
                data.Read(xData, 10);
                for (uint i = 0; i < 10; i++)
                    Debug.Write(xData[i]);
                Debug.Write('\n');
            }*/
            while (true) ;

            while (true)
            {
                Native.Cli();
                Native.Hlt();
            }
        }
    }
}
