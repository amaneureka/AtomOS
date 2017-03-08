/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Conv_U MSIL
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
    [ILOp(ILCode.Conv_U4)]
    public class Conv_U4 : MSIL
    {
        public Conv_U4(Compiler Cmp)
            : base("convu4", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            var xSource = Core.vStack.Pop();

            //Convert to int8, pushing int32 on stack.
            //Mean Just Convert Last 1 byte to Int32
            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        switch (xSource.Size)
                        {
                            case 1:
                            case 2:
                            case 4:
                                {
                                    if (xSource.IsFloat)
                                    {
                                        Core.AssemblerCode.Add(new Cvttss2si { DestinationReg = Registers.EAX, SourceReg = Registers.ESP, SourceIndirect = true });
                                        Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ESP, DestinationIndirect = true, SourceReg = Registers.EAX });
                                    }
                                    else
                                    {
                                        //Not Required because it is already what we want, So Never called
                                    }
                                }
                                break;
                            case 8:
                                {
                                    if (xSource.IsFloat)
                                    {
                                        throw new Exception("Conv_U4 -> Size 8 : Float");
                                    }
                                    else
                                    {
                                        //Just Erase The 8 byte as we want only first 4 byte.
                                        Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                                        Core.AssemblerCode.Add(new Add { DestinationReg = Registers.ESP, SourceRef = "0x4" });
                                        Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EAX });
                                    }
                                }
                                break;

                            default:
                                throw new Exception("Not Yet Implemented Conv_U4 : Size" + xSource.Size);
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

            Core.vStack.Push(4, typeof(UInt32));
        }
    }
}
