/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Stind_Ref MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;
using System.Reflection;

using Atomix.CompilerExt;

namespace Atomix.IL
{
    [ILOp(ILCode.Stind_Ref)]
    public class Stind_Ref : MSIL
    {
        public Stind_Ref(Compiler Cmp)
            : base("stindref", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            switch(ILCompiler.CPUArchitecture)
            {
                case CPUArch.x86:
                    Stind_I.Stind_All(4);
                    break;
                default:
                    throw new Exception("[Stind_Ref]: Not Implemented");
            }
        }
    }
}
