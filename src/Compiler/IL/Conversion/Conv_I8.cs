/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Conv_I8 MSIL
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
    [ILOp(ILCode.Conv_I8)]
    public class Conv_I8 : MSIL
    {
        public Conv_I8(Compiler Cmp)
            : base("convi8", Cmp) { }

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
                                        Core.AssemblerCode.Add(new Fld { DestinationReg = Registers.ESP, DestinationIndirect = true });//fld [ESP]
                                        Core.AssemblerCode.Add(new Sub { DestinationReg = Registers.ESP, SourceRef = "0x4" });//sub ESP, 0x4
                                        Core.AssemblerCode.Add(new Fisttp { DestinationReg = Registers.ESP, DestinationIndirect = true, Size = 64 });//fisttp [ESP]
                                    }
                                    else
                                    {
                                        //First Pop into EAX, convert it to long and push back
                                        Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                                        Core.AssemblerCode.Add(new Conversion { Type = ConversionCode.SignedDWord_2_SignedQWord });
                                        Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EDX });
                                        Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EAX });
                                    }
                                }
                                break;
                            case 8:
                                {
                                    if (xSource.IsFloat)
                                    {
                                        throw new Exception("Conv_I8 -> Size 8 : Float");
                                    }
                                    else
                                    {
                                        //Not Required because it is already what we want, So Never called
                                    }
                                }
                                break;
                            default:
                                throw new Exception("Not Yet Implemented Conv_I8 : Size" + xSource.Size);
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

            Core.vStack.Push(8, typeof(Int64));
        }
    }
}
