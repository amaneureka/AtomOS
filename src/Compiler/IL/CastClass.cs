using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atomix.Assembler;
using Atomix.CompilerExt;
using System.Reflection;

namespace Atomix.IL
{
    [ILOp(ILCode.Castclass)]
    public class CastClass : MSIL
    {
        public CastClass(Compiler Cmp)
            : base("nop", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            //Do Nothing
            //COMPILER HACK ^^
        }
    }
}
