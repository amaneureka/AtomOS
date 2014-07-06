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
using Core = Atomix.Assembler.AssemblyHelper;
using Kernel_alpha.Drivers.Input;

namespace Kernel_alpha
{
    public static class Caller
    {        
        public static Keyboard KBD;

        public static unsafe void Start()
        {
            /* Start MultiTasking */
            Multitasking.CreateTask(0, true); //Our Current System Thread
            Multitasking.CreateTask(pTask1, true);
            Multitasking.CreateTask(pTask2, true);
            Multitasking.Init();
            KBD = new Keyboard();
            PCI.Setup();
            Console.WriteLine("WELCOME TO MY ATOMIX BUILDER");            
        }

        public static unsafe void Update()
        {
            
        }

        private static uint pTask1;
        public static unsafe void Task1()
        {
            byte* xA = (byte*)0xB8000;            
            byte c = 0;
            do
            {
                xA[4] = c;
                xA[5] = 0xb;
                c++;
                if (c >= 255)
                    c = 0;
            }
            while (true);
        }

        private static uint pTask2;
        public static unsafe void Task2()
        {
            byte* xA = (byte*)0xB8000;            
            byte c = 0;
            do
            {
                xA[6] = c;
                xA[7] = 0xd;
                c++;
                if (c >= 255)
                    c = 0;
            }
            while (true);
        }
    }
}
