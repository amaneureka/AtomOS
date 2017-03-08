/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Switch MSIL
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
    [ILOp(ILCode.Switch)]
    public class Switch : MSIL
    {
        public Switch(Compiler Cmp)
            : base("switch", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            var xBranchs = ((OpSwitch)instr).Value;

            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                        for (int i = 0; i < xBranchs.Length; i++)
                        {
                            Core.AssemblerCode.Add(new Cmp { DestinationReg = Registers.EAX, SourceRef = "0x" + i.ToString("X") });
                            string xLabel = ILHelper.GetLabel(aMethod, xBranchs[i]);
                            Core.AssemblerCode.Add(new Jmp { Condition = ConditionalJumpEnum.JE, DestinationRef = xLabel });
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
