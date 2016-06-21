/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
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
