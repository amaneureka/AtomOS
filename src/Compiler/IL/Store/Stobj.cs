/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Stobj MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;
using System.Reflection;

using Atomix.Assembler;
using Atomix.Assembler.x86;
using Atomix.CompilerExt;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.IL
{
    [ILOp(ILCode.Stobj)]
    public class Stobj : MSIL
    {
        public Stobj(Compiler Cmp)
            : base("stobj", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            var xFieldSize = Core.vStack.Pop().Size;
            Core.vStack.Pop();

            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, SourceReg = Registers.ESP, SourceIndirect = true, SourceDisplacement = checked((int)xFieldSize) });
                        for (int i = 0; i < (xFieldSize / 4); i++)
                        {
                            Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, DestinationIndirect = true, DestinationDisplacement = i * 4, SourceReg = Registers.EAX });
                        }
                        switch (xFieldSize % 4)
                        {
                            case 1:
                                {
                                    Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                                    Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, DestinationIndirect = true, DestinationDisplacement = checked((int)(xFieldSize / 4) * 4), SourceReg = Registers.AL });
                                    break;
                                }
                            case 2:
                                {
                                    Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                                    Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, DestinationIndirect = true, DestinationDisplacement = checked((int)(xFieldSize / 4) * 4), SourceReg = Registers.AX });
                                    break;
                                }
                            case 0:
                                {
                                    break;
                                }
                            default:
                                throw new Exception("Remainder size " + (xFieldSize % 4) + " not supported!");
                        }
                        Core.AssemblerCode.Add(new Add { DestinationReg = Registers.ESP, SourceRef = "0x4" });
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
