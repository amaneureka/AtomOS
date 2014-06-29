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
    [ILOp(ILCode.Stind_I2)]
    public class Stind_I2 : MSIL
    {
        public Stind_I2(Compiler Cmp)
            : base("stindi2", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            Stind_I.Stind_All(2);
        }
    }
}
