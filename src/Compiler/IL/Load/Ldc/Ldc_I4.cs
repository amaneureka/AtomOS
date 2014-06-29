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
    [ILOp(ILCode.Ldc_I4)]
    public class Ldc_I4 : MSIL
    {
        public Ldc_I4(Compiler Cmp)
            : base("ldc_i4", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            var xValue = ((OpInt)instr).Value;
            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        Core.AssemblerCode.Add(new Push() { DestinationRef = "0x" + xValue.ToString("X") });
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
            Core.vStack.Push(4, typeof(Int32));
        }
    }
}
