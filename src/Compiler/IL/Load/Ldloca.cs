/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Ldloca MSIL
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
    [ILOp(ILCode.Ldloca)]
    public class Ldloca : MSIL
    {
        public Ldloca(Compiler Cmp)
            : base("ldloca", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            var xVar = ((OpVar)instr).Value;
            var xBody = aMethod.GetMethodBody();
            var xField = xBody.LocalVariables[xVar];
            var xSize = xField.LocalType.SizeOf();
            var StackCount = xSize.Align() / 4;
            var EBPOffset = ILHelper.MemoryOffset(xBody, xVar);

            var xAddress = StackCount* 4 + EBPOffset;
            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP });
                        Core.AssemblerCode.Add(new Sub { DestinationReg = Registers.EAX, SourceRef = "0x" + xAddress.ToString("X") });
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
            Core.vStack.Push(4, xField.LocalType);
        }
    }
}
