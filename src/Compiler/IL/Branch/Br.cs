/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Br MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomix.Assembler.x86;
using Atomix.CompilerExt;
using System.Reflection;
using Atomix.ILOpCodes;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.IL
{
    [ILOp(ILCode.Br)]
    public class Br : MSIL
    {
        public Br(Compiler Cmp)
            : base("br", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            //This is branch type IL
            var xOffset = ((OpBranch)instr).Value;
            //The target branch
            var xTrueLabel = ILHelper.GetLabel(aMethod, xOffset);

            //Just make the jump to given target branch
            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        //Just make a jump as simple as that =)
                        Core.AssemblerCode.Add(new Jmp { DestinationRef = xTrueLabel });
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
