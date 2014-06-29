using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Atomix.CompilerExt;

namespace Atomix.Assembler.x86
{
    public class Call : Instruction        
    {
        public readonly string Address;

        public Call(string Add)
            :base ("call")
        {
            this.Address = Add;
        }

        public override void FlushText(StreamWriter sw)
        {
            sw.WriteLine("call " + Address);
        }
    }
}
