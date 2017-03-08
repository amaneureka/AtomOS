/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          InitObj MSIL
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
    [ILOp(ILCode.Initobj)]
    public class Initobj : MSIL
    {
        public Initobj(Compiler Cmp)
            : base("initobj", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            var mType = ((OpType)instr).Value;
            int mObjSize = mType.SizeOf();

            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                        for (int i = 0; i < (mObjSize / 4); i++)
                        {
                            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, DestinationIndirect = true, DestinationDisplacement = i * 4, SourceRef = "0x0", Size = 32 });
                        }
                        switch (mObjSize % 4)
                        {
                            case 0:
                                break;
                            case 1:
                                {
                                    Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, DestinationIndirect = true, DestinationDisplacement = (int)((mObjSize / 4) * 4), SourceRef = "0x0", Size = 8 });
                                    break;
                                }
                            case 2:
                                {
                                    Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, DestinationIndirect = true, DestinationDisplacement = (int)((mObjSize / 4) * 4), SourceRef = "0x0", Size = 16 });
                                    break;
                                }
                            case 3:
                                {
                                    Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, DestinationIndirect = true, DestinationDisplacement = (int)((mObjSize / 4) * 4), SourceRef = "0x0", Size = 8 });
                                    Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, DestinationIndirect = true, DestinationDisplacement = (int)(((mObjSize / 4) * 4) + 1), SourceRef = "0x0", Size = 16 });
                                    break;
                                }
                            default:
                                    throw new NotImplementedException("@InitObj: Remainder size " + mObjSize % 4 + " not supported yet! (Type = '" + mType.FullName + "')");
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
