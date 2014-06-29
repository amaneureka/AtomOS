using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Atomix.CompilerExt;

namespace Atomix.Assembler
{
    public class Literal : Instruction        
    {        
        public readonly string Assembly;

        public Literal(string aAsm)
            :base ("Literal")
        {
            this.Assembly = aAsm;
        }

        public override void FlushText(StreamWriter sw)
        {
            sw.WriteLine(Assembly);
        }
    }
}
