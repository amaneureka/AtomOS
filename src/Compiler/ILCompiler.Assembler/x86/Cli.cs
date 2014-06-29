using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atomix.Assembler;

namespace Atomix.Assembler.x86
{
    public class Cli : Instruction
    {
        public Cli()
            : base("cli") { }
    }
}
