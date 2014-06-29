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
    [ILOp(ILCode.Ldnull)]
    public class Ldnull : MSIL
    {
        public Ldnull(Compiler Cmp)
            : base("ldnull", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            /*
                Just to push a null object onto stack
            */
            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        //Push 0x0 on stack
                        Core.AssemblerCode.Add(new Push { DestinationRef = "0x0" });
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
            Core.vStack.Push(4, typeof(uint));
        }
    }
}
