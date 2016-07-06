using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;
using Atomix.Assembler;
using Atomix.Assembler.x86;
using Kernel_alpha.x86;
using Kernel_alpha.x86.Intrinsic;
using AssemblyHelper = Atomix.Assembler.AssemblyHelper;

namespace Kernel_alpha
{
    [Kernel(CPUArch.x86, "0x000000")]//Fixed Entrypoint, if you change it than i kill you :)
    public static class Kernel_x86
    {
        [Assembly]
        public static void main()
        {
            const uint MultibootMagic = 0x1BADB002;
            const uint MultibootFlags = 0x10007;
            const uint InitalStackSize = 0x50000;

            /* Multiboot Config */
            AssemblyHelper.AssemblerCode.Add(new Literal("MultibootSignature dd {0}", MultibootMagic));
            AssemblyHelper.AssemblerCode.Add(new Literal("MultibootFlags dd {0}", 65543));
            AssemblyHelper.AssemblerCode.Add(new Literal("MultibootChecksum dd {0}", -(MultibootMagic + MultibootFlags)));
            AssemblyHelper.AssemblerCode.Add(new Literal("MultibootHeaderAddr dd {0}", 0));
            AssemblyHelper.AssemblerCode.Add(new Literal("MultibootLoadAddr dd {0}", 0));
            AssemblyHelper.AssemblerCode.Add(new Literal("MultibootLoadEndAddr dd {0}", 0));
            AssemblyHelper.AssemblerCode.Add(new Literal("MultibootBSSEndAddr dd {0}", 0));
            AssemblyHelper.AssemblerCode.Add(new Literal("MultibootEntryAddr dd {0}", 0));

            AssemblyHelper.InsertData(new AsmData("InitialStack", InitalStackSize));

            AssemblyHelper.AssemblerCode.Add(new Label("_Kernel_Main"));

            /* Here is Entrypoint Method */
            AssemblyHelper.AssemblerCode.Add(new Cli()); //Clear interrupts first !!

            //Setup Stack pointer, We do rest things later (i.e. Another method) because they are managed :)
            AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.ESP, SourceRef = "InitialStack" });
            AssemblyHelper.AssemblerCode.Add(new Add { DestinationReg = Registers.ESP, SourceRef = InitalStackSize.ToString() });

            AssemblyHelper.AssemblerCode.Add(new Push { DestinationReg = Registers.EAX });
            AssemblyHelper.AssemblerCode.Add(new Push { DestinationReg = Registers.EBX });//Push Multiboot Header Info Address
            AssemblyHelper.AssemblerCode.Add(new Call ("Kernel_Start"));
        }

        [Plug("Kernel_Start")]
        public static unsafe void Start (uint magic, uint address)
        {
            /* Setup Multiboot */
            Multiboot.Setup(magic, address);

            /* Placement Address */
            Heap.PlacementAddress = Native.EndOfKernel();

            /* Clear Interrupts */
            Native.ClearInterrupt();

            /* Setup PIC */
            PIC.Setup();

            /* Setup GDT & Enter into protected mode */
            GDT.Setup();

            /* Setup IDT */
            IDT.Setup();

            /* Enable Interrupts */
            Native.SetInterrupt();

            /* Setup Paging */
            Paging.Setup(Multiboot.RAM);

            /* Setup Multitasking */
            Multitasking.CreateTask(0, true); //This is System Update thread
            Multitasking.Init();//Start Multitasking

            /* Call our kernel instance now */
            try
            {
                Caller.Start();
                while(true)
                {
                    Caller.Update();
                }
            }
            catch (Exception e)
            {
                //Kernel PANIC !!
                Console.WriteLine(e.Message);
            }

            while (true)  //Set CPU in Infinite loop DON'T REMOVE THIS ELSE I'll KILL YOU (^ . ^)
            {
                Native.ClearInterrupt();
                Native.Halt();
            };
        }
    }
}
