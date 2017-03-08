/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Ldobj MSIL
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
    [ILOp(ILCode.Ldobj)]
    public class Ldobj : MSIL
    {
        public Ldobj(Compiler Cmp)
            : base("ldobj", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            var xType = ((OpType)instr).Value;
            var xSize = Core.vStack.Pop();
            var xObjSize = ILHelper.StorageSize(xType);

            /*
                The address of a value type object is pushed onto the stack.
                The address is popped from the stack and the instance at that particular address is looked up.
                The value of the object stored at that address is pushed onto the stack.
            */
            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });

                        for (int i = 1; i <= (xObjSize / 4); i++)
                        {
                            Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EAX, DestinationIndirect = true, DestinationDisplacement = (int)(xObjSize - (i * 4)) });
                        }

                        switch (xObjSize % 4)
                        {
                            case 1:
                                {
                                    Core.AssemblerCode.Add(new Xor { DestinationReg = Registers.EBX, SourceReg = Registers.EBX });
                                    Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.BL, SourceIndirect = true, SourceReg = Registers.EAX, Size = 8 });
                                    Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EBX });
                                    break;
                                }
                            case 2:
                                {
                                    Core.AssemblerCode.Add(new Xor { DestinationReg = Registers.EBX, SourceReg = Registers.EBX });
                                    Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.BX, SourceIndirect = true, SourceReg = Registers.EAX, Size = 16 });
                                    Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EBX });
                                    break;
                                }
                            case 0:
                                {
                                    break;
                                }
                            default:
                                throw new Exception("@Ldobj: Remainder not supported!");
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

            Core.vStack.Push(xObjSize, xType);
        }
    }
}
