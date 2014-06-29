using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atomix.Assembler;
using Atomix.CompilerExt;
using System.Reflection;

namespace Atomix.IL
{
    [ILOp(ILCode.Nop)]
    public class Nop : MSIL
    {
        /* Do nothing (No operation).*/
        public Nop(Compiler Cmp)
            : base("nop", Cmp) { }
    }
}
