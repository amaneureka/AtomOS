/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Ldloc MSIL
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
    [ILOp(ILCode.Ldloc)]
    public class Ldloc : MSIL
    {
        public Ldloc(Compiler Cmp)
            : base("ldloc", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            var xVar = ((OpVar)instr).Value;
            var xBody = aMethod.GetMethodBody();
            var xField = xBody.LocalVariables[xVar];
            var xSize = xField.LocalType.SizeOf();
            var StackCount = xSize.Align() / 4;
            var EBPOffset = ILHelper.MemoryOffset(xBody, xVar);

            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        switch (xSize)
                        {
                            case 1:
                            case 2:
                                {
                                    bool IsSigned = xField.LocalType.IsSigned();
                                    if (IsSigned)
                                    {
                                        Core.AssemblerCode.Add(new Movsx
                                        {
                                            DestinationReg = Registers.EAX,
                                            SourceReg = Registers.EBP,
                                            SourceIndirect = true,
                                            SourceDisplacement = 0 - (int)(EBPOffset + 4),
                                            Size = 0x10
                                        });
                                        Core.AssemblerCode.Add(new Push() { DestinationReg = Registers.EAX });
                                    }
                                    else
                                    {
                                        Core.AssemblerCode.Add(new Movzx
                                        {
                                            DestinationReg = Registers.EAX,
                                            SourceReg = Registers.EBP,
                                            SourceIndirect = true,
                                            SourceDisplacement = 0 - (int)(EBPOffset + 4),
                                            Size = 0x10
                                        });
                                        Core.AssemblerCode.Add(new Push() { DestinationReg = Registers.EAX });
                                    }
                                }
                                break;
                            case 4:
                                {
                                    Core.AssemblerCode.Add(new Push
                                    {
                                        DestinationReg = Registers.EBP,
                                        DestinationIndirect = true,
                                        DestinationDisplacement = 0 - (int)(EBPOffset + 4),
                                    });
                                }
                                break;
                            default:
                                {
                                    for (int i = StackCount; i > 0; i--)
                                    {
                                        //push dword [EBP - 0x*]
                                        Core.AssemblerCode.Add(new Push
                                        {
                                            DestinationReg = Registers.EBP,
                                            DestinationIndirect = true,
                                            DestinationDisplacement = 0 - (int)(EBPOffset + i * 4),
                                        });
                                    }
                                }
                                break;
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
            Core.vStack.Push(xSize.Align(), xField.LocalType);
        }
    }
}
