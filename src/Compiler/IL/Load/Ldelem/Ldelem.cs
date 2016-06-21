/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Ldelem MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System.Reflection;

using Atomix.CompilerExt;
using Atomix.ILOpCodes;

namespace Atomix.IL
{
    [ILOp(ILCode.Ldelem)]
    public class Ldelem : MSIL
    {
        public Ldelem(Compiler Cmp)
            : base("ldelem", Cmp) { }

        
        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            var xOperand = ((OpType)instr).Value;
            var xSize = xOperand.SizeOf();

            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        Ldelem_Ref.Ldelem_x86(xSize, false);
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
