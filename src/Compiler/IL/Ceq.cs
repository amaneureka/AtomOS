/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Ceq MSIL
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
    [ILOp(ILCode.Ceq)]
    public class Ceq : MSIL
    {
        public Ceq(Compiler Cmp)
            : base("ceq", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            var FirstStackItem = Core.vStack.Pop();
            var SecondStackItem = Core.vStack.Pop();

            var xSize = Math.Max(FirstStackItem.Size, SecondStackItem.Size);
            var xCurrentLabel = ILHelper.GetLabel(aMethod, instr.Position);
            var xFinalLabel = ILHelper.GetLabel(aMethod, instr.NextPosition);

            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        /*Ceq checks if both values in stack are equal or not, if they are equal then they push 1 onto the stack else
                         * 0 is pushed to the stack
                         */
                        switch (xSize)
                        {
                            case 1:
                            case 2:
                            case 4:
                                {
                                    if (FirstStackItem.IsFloat) //If one item is float then both will
                                    {
                                        Core.AssemblerCode.Add(new Movss { DestinationReg = Registers.XMM0, SourceReg = Registers.ESP, SourceIndirect = true });
                                        Core.AssemblerCode.Add(new Assembler.x86.Add { DestinationReg = Registers.ESP, SourceRef = "0x4" });
                                        Core.AssemblerCode.Add(new Movss { DestinationReg = Registers.XMM1, SourceReg = Registers.ESP, SourceIndirect = true });
                                        Core.AssemblerCode.Add(new Cmpss { DestinationReg = Registers.XMM0, SourceReg = Registers.XMM1, PseudoCode = ComparePseudoOpcodes.Equal });
                                        Core.AssemblerCode.Add(new MovD { SourceReg = Registers.XMM0, DestinationReg = Registers.EBX });
                                        Core.AssemblerCode.Add(new And { DestinationReg = Registers.EBX, SourceRef = "0x1" });
                                        Core.AssemblerCode.Add(new Mov { SourceReg = Registers.EBX, DestinationReg = Registers.ESP, DestinationIndirect = true });
                                    }
                                    else
                                    {
                                        Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                                        Core.AssemblerCode.Add(new Cmp { DestinationReg = Registers.EAX, SourceReg = Registers.ESP, SourceIndirect = true });
                                        Core.AssemblerCode.Add(new Jmp { Condition = ConditionalJumpEnum.JE, DestinationRef = xCurrentLabel + ".true" });
                                        Core.AssemblerCode.Add(new Assembler.x86.Add { DestinationReg = Registers.ESP, SourceRef = "0x4" });
                                        //Not equal
                                        Core.AssemblerCode.Add(new Push { DestinationRef = "0x0" });
                                        Core.AssemblerCode.Add(new Jmp { DestinationRef = xFinalLabel });

                                        //equal
                                        Core.AssemblerCode.Add(new Label(xCurrentLabel + ".true"));
                                        Core.AssemblerCode.Add(new Assembler.x86.Add { DestinationReg = Registers.ESP, SourceRef = "0x4" });
                                        Core.AssemblerCode.Add(new Push { DestinationRef = "0x1" });
                                        Core.AssemblerCode.Add(new Jmp { DestinationRef = xFinalLabel });
                                        Core.AssemblerCode.Add(new Label(xFinalLabel));
                                    }
                                }
                                break;
                            case 8:
                                {
                                    if (FirstStackItem.IsFloat)
                                    {
                                    }
                                    else
                                    {
                                        //Check Hight Part
                                        Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                                        Core.AssemblerCode.Add(new Cmp { DestinationReg = Registers.EAX, SourceReg = Registers.ESP, SourceIndirect = true, SourceDisplacement = 0x4 });
                                        Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                                        Core.AssemblerCode.Add(new Jmp { Condition = ConditionalJumpEnum.JNE, DestinationRef = xCurrentLabel + ".false" });

                                        //Check Low part
                                        Core.AssemblerCode.Add(new Xor { DestinationReg = Registers.EAX, SourceReg = Registers.ESP, SourceIndirect = true, SourceDisplacement = 0x4 });
                                        Core.AssemblerCode.Add(new Jmp { Condition = ConditionalJumpEnum.JNZ, DestinationRef = xCurrentLabel + ".false" });

                                        //equal
                                        Core.AssemblerCode.Add(new Assembler.x86.Add { DestinationReg = Registers.ESP, SourceRef = "0x8" });
                                        Core.AssemblerCode.Add(new Push { DestinationRef = "0x1" });
                                        Core.AssemblerCode.Add(new Jmp { DestinationRef = xFinalLabel });

                                        //not equal
                                        Core.AssemblerCode.Add(new Label(xCurrentLabel + ".false"));
                                        Core.AssemblerCode.Add(new Assembler.x86.Add { DestinationReg = Registers.ESP, SourceRef = "0x8" });
                                        Core.AssemblerCode.Add(new Push { DestinationRef = "0x0" });
                                        Core.AssemblerCode.Add(new Jmp { DestinationRef = xFinalLabel });
                                        Core.AssemblerCode.Add(new Label(xFinalLabel));
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
            Core.vStack.Push(4, typeof(bool));
        }
    }
}
