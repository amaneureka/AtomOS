/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Shl MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System.Reflection;

using Atomix.Assembler;
using Atomix.CompilerExt;
using Atomix.Assembler.x86;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.IL
{
    [ILOp(ILCode.Shl)]
    public class ILShl : MSIL
    {
        public ILShl(Compiler Cmp)
            : base("shl", Cmp)
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

                            // x.High = (x.High << y) | (x.Low & ~((1 << y) - 1))
                            // x.Low = x.Low << y
                            Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.ECX });

                            // low
                            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.ESP, SourceIndirect = true });

                            Core.AssemblerCode.Add(new Cmp { DestinationReg = Registers.ECX, SourceRef = "0x20" });
                            Core.AssemblerCode.Add(new Jmp { Condition = ConditionalJumpEnum.JL, DestinationRef = ".LessThan32" });
                            // low
                            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ESP, DestinationIndirect = true, SourceRef = "0x0" });
                            // high
                            Core.AssemblerCode.Add(new And { DestinationReg = Registers.ECX, SourceRef = "0x1F" });
                            Core.AssemblerCode.Add(new ShiftLeft { DestinationReg = Registers.EAX, SourceReg = Registers.CL });
                            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ESP, DestinationIndirect = true, DestinationDisplacement = 4, SourceReg = Registers.EAX });
                            Core.AssemblerCode.Add(new Jmp { DestinationRef = ".ShlComplete" });

                            Core.AssemblerCode.Add(new Label(".LessThan32"));
                            // High
                            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EDX, SourceReg = Registers.ESP, SourceDisplacement = 4, SourceIndirect = true });
                            // shift lower
                            Core.AssemblerCode.Add(new ShiftLeft { DestinationReg = Registers.ESP, DestinationIndirect = true, SourceReg = Registers.CL });
                            // shift high
                            Core.AssemblerCode.Add(new Literal("shld dword EDX, EAX, CL"));
                            Core.AssemblerCode.Add(new Label(".ShlComplete"));
                        }
                        else
                        {
                            Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.ECX }); //Shift Amount
                            Core.AssemblerCode.Add(new ShiftLeft { DestinationReg = Registers.ESP, DestinationIndirect = true, SourceReg = Registers.CL });
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
