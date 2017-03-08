/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Starg MSIL
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
    [ILOp(ILCode.Starg)]
    public class Starg : MSIL
    {
        public Starg(Compiler Cmp)
            : base("starg", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            var xOpVar = (OpVar)instr;

            var xMethodInfo = aMethod as System.Reflection.MethodInfo;
            int xReturnSize = 0;
            if (xMethodInfo != null)
            {
                xReturnSize = xMethodInfo.ReturnType.SizeOf().Align();
            }

            int xOffset = 8;
            var xCorrectedOpValValue = xOpVar.Value;
            if (!aMethod.IsStatic && xOpVar.Value > 0)
            {
                xCorrectedOpValValue -= 1;
            }
            var xParams = aMethod.GetParameters();

            for (int i = xParams.Length - 1; i > xCorrectedOpValValue; i--)
            {
                var xSize = xParams[i].ParameterType.SizeOf().Align();
                xOffset += xSize;
            }

            var xCurArgSize = xParams[xCorrectedOpValValue].ParameterType.SizeOf().Align();

            int xArgSize = 0;
            foreach (var xParam in xParams)
            {
                xArgSize += xParam.ParameterType.SizeOf().Align();
            }

            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        for (int i = 0; i < (xCurArgSize / 4); i++)
                        {
                            Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBP, DestinationIndirect = true, DestinationDisplacement = (int)(xOffset + ((i) * 4)), SourceReg = Registers.EAX });
                        }
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
            Core.vStack.Pop();
        }
    }
}
