/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Stind_I MSIL
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
    [ILOp(ILCode.Stind_I)]
    public class Stind_I : MSIL
    {
        public Stind_I(Compiler Cmp)
            : base("stindi", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            /*
             * So, Situation is we have two items on stack,
             * Last one is value and before that is its address where it should be saved
             * Stind_I -> The Size of item is native int => CPU == x86 ? 4 : 8
             *
             */
            if (ILCompiler.CPUArchitecture == CompilerExt.CPUArch.x86)
                Stind_All(4);
            else if (ILCompiler.CPUArchitecture == CompilerExt.CPUArch.x64)
                Stind_All(8);

        }
        public static void Stind_All(int xSize)
        {
            var ValueStackCount = xSize.Align();

            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        Core.AssemblerCode.Add(
                            new Mov {
                                DestinationReg = Registers.EBX,
                                SourceReg = Registers.ESP,
                                SourceIndirect = true,
                                SourceDisplacement = ValueStackCount });

                        for (int i = 0; i < xSize/4; i++)
                        {
                            Core.AssemblerCode.Add(
                            new Mov
                            {
                                DestinationReg = Registers.EAX,
                                SourceReg = Registers.ESP,
                                SourceIndirect = true,
                                SourceDisplacement = i * 4
                            });
                            Core.AssemblerCode.Add(
                            new Mov
                            {
                                DestinationReg = Registers.EBX,
                                DestinationIndirect = true,
                                DestinationDisplacement = i * 4,
                                SourceReg = Registers.EAX
                            });
                        }

                        switch (xSize % 4)
                        {
                            case 0:
                                break;
                            case 1:
                                {
                                    Core.AssemblerCode.Add(
                                        new Mov
                                        {
                                            DestinationReg = Registers.EAX,
                                            SourceReg = Registers.ESP,
                                            SourceIndirect = true,
                                            SourceDisplacement = (int)(xSize / 4) * 4
                                        });
                                    Core.AssemblerCode.Add(
                                        new Mov
                                        {
                                            DestinationReg = Registers.EBX,
                                            DestinationIndirect = true,
                                            SourceDisplacement = (int)(xSize / 4) * 4,
                                            SourceReg = Registers.AL,
                                            Size = 8
                                        });
                                }
                                break;
                            case 2:
                                {
                                    Core.AssemblerCode.Add(
                                        new Mov
                                        {
                                            DestinationReg = Registers.EAX,
                                            SourceReg = Registers.ESP,
                                            SourceIndirect = true,
                                            SourceDisplacement = (int)(xSize / 4) * 4
                                        });
                                    Core.AssemblerCode.Add(
                                        new Mov
                                        {
                                            DestinationReg = Registers.EBX,
                                            DestinationIndirect = true,
                                            SourceDisplacement = (int)(xSize / 4) * 4,
                                            SourceReg = Registers.AX,
                                            Size = 16
                                        });
                                }
                                break;
                            default:
                                throw new Exception("Unknown Size");
                        }
                        //shift ESP to new address
                        Core.AssemblerCode.Add( new Add { DestinationReg = Registers.ESP, SourceRef = "0x" + (ValueStackCount + 4).ToString("X") });
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
            Core.vStack.Pop(); //Value
            Core.vStack.Pop(); //Address
        }
    }
}
