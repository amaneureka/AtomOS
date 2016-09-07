/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Ldelem_R4 MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System.Reflection;

using Atomix.CompilerExt;

namespace Atomix.IL
{
    [ILOp(ILCode.Ldelem_R4)]
    public class Ldelem_R4 : MSIL
    {
        public Ldelem_R4(Compiler Cmp)
            : base("ldelemr4", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        Ldelem_Ref.Ldelem_x86(4, false);
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
