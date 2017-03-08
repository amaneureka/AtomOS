/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Ldc_R4 MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;
using System.Reflection;

using Atomix.Assembler.x86;
using Atomix.CompilerExt;
using Atomix.ILOpCodes;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.IL
{
    [ILOp(ILCode.Ldc_R4)]
    public class Ldc_R4 : MSIL
    {
        public Ldc_R4(Compiler Cmp)
            : base("ldc_r4", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            var xValue = ((OpSingle)instr).Value;
            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        //Bit Converter is used instead of direact ToString because, ToString throws format expection on that
                        //Format specifier was invalid.
                        var xData = BitConverter.GetBytes(xValue);
                        Core.AssemblerCode.Add(new Push() { DestinationRef = "0x" + BitConverter.ToUInt32(xData, 0).ToString("X") });
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
            Core.vStack.Push(4, typeof(float));
        }
    }
}
