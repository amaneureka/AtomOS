/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Leave MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System.Reflection;

using Atomix.Assembler.x86;
using Atomix.CompilerExt;
using Atomix.ILOpCodes;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.IL
{
    [ILOp(ILCode.Leave)]
    public class ILLeave : MSIL
    {
        public ILLeave(Compiler Cmp)
            : base("leave", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            var xOffset = ((OpBranch)instr).Value;
            var xAddr = ILHelper.GetLabel(aMethod, xOffset);

            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        Core.AssemblerCode.Add(new Jmp { DestinationRef = xAddr });
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
