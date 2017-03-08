/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Stfld MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;
using System.Reflection;

using Atomix.Assembler;
using Atomix.Assembler.x86;
using Atomix.CompilerExt;
using Atomix.ILOpCodes;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.IL
{
    [ILOp(ILCode.Stfld)]
    public class Stfld : MSIL
    {
        public Stfld(Compiler Cmp)
            : base("stfld", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            var xF = ((OpField)instr).Value;
            var aDeclaringType = xF.DeclaringType;
            var xFieldId = xF.FullName();

            FieldInfo xFieldInfo = null;

            //Now we have to calculate the offset of object, and also give us that field
            int xOffset = ILHelper.GetFieldOffset(aDeclaringType, xFieldId, out xFieldInfo);
            bool xNeedsGC = aDeclaringType.IsClass && !aDeclaringType.IsValueType;
            if (xNeedsGC)
                xOffset += 12; //Extra offset =)

            //As we are sure xFieldInfo should contain value as if not than it throws error in GetFieldOffset
            var xSize = xFieldInfo.FieldType.SizeOf();
            var xRoundedSize = xSize.Align();

            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {

                        Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBX, SourceReg = Registers.ESP, SourceIndirect = true, SourceDisplacement = (int)xRoundedSize });
                        Core.AssemblerCode.Add(new Add { DestinationReg = Registers.EBX, SourceRef = "0x" + xOffset.ToString("X") });

                        for (int i = 0; i < (xSize / 4); i++)
                        {
                            Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBX, DestinationIndirect = true, DestinationDisplacement = (int)((i * 4)), SourceReg = Registers.EAX });
                        }

                        switch (xSize % 4)
                        {
                            case 1:
                                {
                                    Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                                    Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBX, DestinationIndirect = true, DestinationDisplacement = (int)((xSize / 4) * 4), SourceReg = Registers.AL, Size = 8 });
                                    break;
                                }
                            case 2:
                                {
                                    Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                                    Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBX, DestinationIndirect = true, DestinationDisplacement = (int)((xSize / 4) * 4), SourceReg = Registers.AX, Size = 16 });
                                    break;
                                }
                            case 3:
                                {
                                    Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                                    Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBX, DestinationIndirect = true, DestinationDisplacement = (int)((xSize / 4) * 4), SourceReg = Registers.AX, Size = 16 });
                                    Core.AssemblerCode.Add(new ShiftRight { DestinationReg = Registers.EAX, SourceRef = "0x10" });
                                    Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBX, DestinationIndirect = true, DestinationDisplacement = (int)((xSize / 4) * 4) + 2, SourceReg = Registers.AL, Size = 8 });
                                    break;
                                }
                            case 0:
                                {
                                    break;
                                }
                            default:
                                throw new Exception("@Stfld: Remainder size " + (xSize % 4) + " not supported!");
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

            Core.vStack.Pop();
        }
    }
}
