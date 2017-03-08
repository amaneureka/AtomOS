/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Ldc_I8 MSIL
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
    [ILOp(ILCode.Ldc_I8)]
    public class Ldc_I8 : MSIL
    {
        public Ldc_I8(Compiler Cmp)
            : base("ldc_i8", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            var xValue = ((OpInt64)instr).Value;
            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        //Note: x86 arch. takes numbers in little endian, so 0xAABB is pushed as BB AA
                        //Mean high bit is pushed first then low bit
                        //We have xValue like 0xAABBCCDDEEFF1122, byte array will be 22 11 FF EE DD CC BB AA
                        //High part from offset 4, i.e. DD CC BB AA = 0xAABBCCDD
                        //Low part from offset 0, i.e. 22 11 FF EE = 0xEEFF1122
                        var xData = BitConverter.GetBytes(xValue);
                        //High bit
                        Core.AssemblerCode.Add(new Push() { DestinationRef = "0x" + BitConverter.ToUInt32(xData, 4).ToString("X") });
                        //Low bit
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
            Core.vStack.Push(8, typeof(Int64));
        }
    }
}
