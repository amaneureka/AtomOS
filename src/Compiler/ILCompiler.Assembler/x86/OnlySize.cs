using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Atomix.CompilerExt;

namespace Atomix.Assembler.x86
{
    public abstract class OnlySize : Instruction
    {       
        public byte Address;

        public OnlySize(string aM)
            : base(aM)  { }

        public override void FlushText(StreamWriter sw)
        {   
            sw.WriteLine(string.Format("{0} 0x{1}", Code, Address.ToString("X")));
        }
    }
}
