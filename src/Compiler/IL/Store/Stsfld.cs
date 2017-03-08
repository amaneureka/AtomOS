/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Stsfld MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;
using System.Linq;
using System.Reflection;

using Atomix.Assembler;
using Atomix.Assembler.x86;
using Atomix.CompilerExt;
using Atomix.ILOpCodes;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.IL
{
    [ILOp(ILCode.Stsfld)]
    public class Stsfld : MSIL
    {
        public Stsfld(Compiler Cmp)
            : base("stsfld", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            var xOperand = ((OpField)instr);
            var xTargetField = xOperand.Value;
            var xTargetType = xTargetField.DeclaringType;
            var xSize = xTargetField.FieldType.SizeOf();

            var xFieldEntry =  xTargetField.FullName();

            var xEndException = aMethod.FullName() + ".Error";
            if (instr.Ehc != null && instr.Ehc.HandlerOffset > instr.Position)
            {
                xEndException = ILHelper.GetLabel(aMethod, instr.Ehc.HandlerOffset);
            }

            string xCctorAddress = null;
            if (aMethod != null)
            {
                var xCctor = (xTargetType.GetConstructors(BindingFlags.Static | BindingFlags.NonPublic) ?? new ConstructorInfo[0]).SingleOrDefault();
                if (xCctor != null)
                    xCctorAddress = xCctor.FullName();
            }

            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        if (xCctorAddress != null)
                        {
                            Core.AssemblerCode.Add(new Call(xCctorAddress));
                            Core.AssemblerCode.Add(new Test { DestinationReg = Registers.ECX, SourceRef = "0x2" });
                            Core.AssemblerCode.Add(new Jmp { Condition = ConditionalJumpEnum.JNE, DestinationRef = xEndException });
                        }

                        if (xSize >= 4)
                        {
                            for (int i = 0; i < xSize / 4; i++)
                            {
                                Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                                Core.AssemblerCode.Add(new Mov { DestinationRef = xFieldEntry, DestinationDisplacement = i * 4, DestinationIndirect = true, SourceReg = Registers.EAX });
                            }
                        }

                        switch (xSize % 4)
                        {
                            case 0:
                                {
                                    break;
                                }
                            case 1:
                                {
                                    Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                                    Core.AssemblerCode.Add(
                                        new Mov
                                        {
                                            DestinationRef = xFieldEntry,
                                            DestinationIndirect = true,
                                            DestinationDisplacement = ((int)(xSize/4))*4,
                                            SourceReg = Registers.AL,
                                            Size = 8
                                        });
                                }
                                break;
                            case 2:
                                {
                                    Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                                    Core.AssemblerCode.Add(
                                        new Mov
                                        {
                                            DestinationRef = xFieldEntry,
                                            DestinationIndirect = true,
                                            DestinationDisplacement = ((int)(xSize / 4)) * 4,
                                            SourceReg = Registers.AX,
                                            Size = 16
                                        });
                                }
                                break;
                            default:
                                    throw new Exception("Unknown Size");
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
