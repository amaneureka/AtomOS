using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Atomix.CompilerExt;
using Atomix.Assembler;

namespace Atomix.Assembler.x86
{
    public class Leave : Instruction
    {
        public Leave()
            : base("leave") { }

        public override void FlushText(StreamWriter sw)
        {
            sw.WriteLine("leave");
        }
             
    }
}
