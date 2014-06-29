using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atomix.IL;
using Atomix.Assembler;
using Atomix.Assembler.x86;
using Atomix.CompilerExt;
using System.Reflection;
using Atomix.ILOpCodes;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.IL
{
    [ILOp(ILCode.Stind_I1)]
    public class Stind_I1 : MSIL
    {
        public Stind_I1(Compiler Cmp)
            : base("stindi1", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {

            Stind_I.Stind_All(1);
        }
    }
}
