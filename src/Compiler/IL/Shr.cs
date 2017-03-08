/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Shr MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System.Reflection;

using Atomix.Assembler;
using Atomix.CompilerExt;
using Atomix.Assembler.x86;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.IL
{
    [ILOp(ILCode.Shr)]
    public class ILShr : MSIL
    {
        public ILShr(Compiler Cmp)
            : base("shr", Cmp)
        { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            var xStackItem_ShiftAmount = Core.vStack.Pop();
            var xStackItem_Value = Core.vStack.Peek();

            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        if (xStackItem_Value.Size > 4)
                        {
                            // stack item 1 - arg (x)
                            // stack item 2 - shift amount (y)

                            // x.Low = (x.Low >> y) | (x.High & ((1 << y) - 1)) << ((1 << (32 - y)) - 1)
                            // x.High = (x.High >> y)
                            Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.ECX });

                            // high
                            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.ESP, SourceDisplacement = 4, SourceIndirect = true });

                            Core.AssemblerCode.Add(new Cmp { DestinationReg = Registers.ECX, SourceRef = "0x20" });
                            Core.AssemblerCode.Add(new Jmp { Condition = ConditionalJumpEnum.JL, DestinationRef = ".LessThan32" });
                            // high
                            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ESP, DestinationDisplacement = 4, DestinationIndirect = true, SourceRef = "0x0" });
                            // low
                            Core.AssemblerCode.Add(new And { DestinationReg = Registers.ECX, SourceRef = "0x1F" });
                            Core.AssemblerCode.Add(new ShiftRight { DestinationReg = Registers.EAX, SourceReg = Registers.CL });
                            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ESP, DestinationIndirect = true, DestinationDisplacement = 4, SourceReg = Registers.EAX });
                            Core.AssemblerCode.Add(new Jmp { DestinationRef = ".ShrComplete" });

                            Core.AssemblerCode.Add(new Label(".LessThan32"));
                            // low
                            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EDX, SourceReg = Registers.ESP, SourceIndirect = true });
                            // shift high
                            Core.AssemblerCode.Add(new ShiftRight { DestinationReg = Registers.ESP, DestinationIndirect = true, DestinationDisplacement = 4, SourceReg = Registers.CL });
                            // shift low
                            Core.AssemblerCode.Add(new Literal("shrd dword EDX, EAX, CL"));
                            Core.AssemblerCode.Add(new Label(".ShrComplete"));
                        }
                        else
                        {
                            Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.ECX }); // Shift Amount
                            Core.AssemblerCode.Add(new ShiftRight { DestinationReg = Registers.ESP, DestinationIndirect = true, SourceReg = Registers.CL });
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
        }
    }
}
