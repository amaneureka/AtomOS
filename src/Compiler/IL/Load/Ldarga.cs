/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Ldarga MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System.Reflection;

using Atomix.Assembler;
using Atomix.Assembler.x86;
using Atomix.CompilerExt;
using Atomix.ILOpCodes;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.IL
{
    [ILOp(ILCode.Ldarga)]
    public class Ldarga : MSIL
    {
        public Ldarga(Compiler Cmp)
            : base("ldarga", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            var aParam = ((OpVar)instr).Value;
            var xDisplacement = ILHelper.GetArgumentDisplacement(aMethod, aParam);
            
            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBX, SourceRef = "0x" + xDisplacement.ToString("X") });
                        Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP });
                        Core.AssemblerCode.Add(new Add { DestinationReg = Registers.EAX, SourceReg = Registers.EBX });
                        Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EAX });
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
