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
    [ILOp(ILCode.Stelem)]
    public class Stelem : MSIL
    {
        public Stelem(Compiler Cmp)
            : base("stelem", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            var xOp = ((OpType)instr).Value;

            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        Stelem_Ref.Stelem_x86(this.Compiler, instr, aMethod, xOp.SizeOf());
                    }
                    break;
                #endregion
                #region _x64_
                case CPUArch.x64:
                    {

                    }
                    break;
                #endregion
                #region _ARM_
                case CPUArch.ARM:
                    {

                    }
                    break;
                #endregion
            }
        }
    }
}
