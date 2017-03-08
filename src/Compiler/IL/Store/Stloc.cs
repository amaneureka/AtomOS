/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Stloc MSIL
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
    [ILOp(ILCode.Stloc)]
    public class Stloc : MSIL
    {
        public Stloc(Compiler Cmp)
            : base("stloc", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            var xVar = ((OpVar)instr).Value;
            var xBody = aMethod.GetMethodBody();
            //So, we have to pop the stack value into EAX
            //save it as local variable into memory (EBP-X)
            var xField = xBody.LocalVariables[xVar];
            //No. of stack count = xField.SizeOf().Align()/4;
            //POP each stack into eax and mov it to memory, then do same for other stack value,
            //till all xField stack is not popped up and stores in memory
            //EBP Offset => 0 - (4 + size of each variable before this)
            var StackCount = xField.LocalType.SizeOf().Align() / 4;
            var EBPOffset = ILHelper.MemoryOffset(xBody, xVar);

            for (int i = StackCount; i > 0; i-- )
            {
                switch (ILCompiler.CPUArchitecture)
                {
                    #region _x86_
                    case CPUArch.x86:
                        {
                            Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                            Core.AssemblerCode.Add(new Mov
                            {
                                DestinationReg = Registers.EBP,
                                DestinationIndirect = true,
                                DestinationDisplacement = 0 - (int)(EBPOffset + i * 4),
                                SourceReg = Registers.EAX
                            });
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
            Core.vStack.Pop();
        }
    }
}
