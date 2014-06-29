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
    [ILOp(ILCode.Sizeof)]
    public class SizeOf : MSIL
    {
        public SizeOf(Compiler Cmp)
            : base("sizeof", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            var xOp = ((OpType)instr).Value;
            var xSize = xOp.SizeOf();

            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        Core.AssemblerCode.Add(new Push { DestinationRef = "0x" + xSize.ToString("X") });
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
            Core.vStack.Push(4, typeof(UInt32));
        }
    }
}
