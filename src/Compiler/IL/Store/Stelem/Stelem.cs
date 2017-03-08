/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Stelem MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System.Reflection;

using Atomix.CompilerExt;
using Atomix.ILOpCodes;

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
