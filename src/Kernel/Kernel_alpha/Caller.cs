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

namespace Kernel_alpha
{
    public static class Caller
    {
        private static Thread xNull;
        public static unsafe void Start()
        {
            Console.WriteLine("Testing started...");
            //Create a system update thread
            var s = Multitasking.CreateTask(0);
            xNull = Multitasking.CreateTask(pTest);
            s.Start();
            xNull.Start();
            Multitasking.Init();
            Console.WriteLine("Testing started...2");
        }
        private static byte xCurr = 0x0;
        public static unsafe void Update()
        {
            byte* xA = (byte*)0xB8000;
            xA[3] = 0xC;
            xA[2] = xCurr;
            xCurr++;
            if (xCurr >= 255)
                xCurr = 0;
        }

        public static uint pTest;
        public static unsafe void Test()
        {
            byte* xA = (byte*)0xB8000;
            xA[1] = 0xA;
            byte x = 0x0;
            uint c = 0;
            do
            {
                x++;
                c++;
                xA[0] = x;
                if (x >= 255)
                    x = 0;
            }
            while (c == 25500);
            Console.WriteLine("My Task is over");
        }
    }
}
