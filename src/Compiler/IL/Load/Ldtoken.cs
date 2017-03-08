/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Ldtoken MSIL
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
    [ILOp(ILCode.Ldtoken)]
    public class Ldtoken : MSIL
    {
        public Ldtoken(Compiler Cmp)
            : base("ldtoken", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            var xToken = (OpToken)instr;
            string xTokenAddress = null;

            if (xToken.ValueIsType)
            {
                xTokenAddress = ILHelper.GetTypeIDLabel(xToken.ValueType);
            }

            if (xToken.ValueIsField)
            {
                xTokenAddress = xToken.ValueField.FullName();
            }

            if (String.IsNullOrEmpty(xTokenAddress))
                throw new Exception("@Ldtoken: xToken Address Can't be null");

            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        Core.AssemblerCode.Add(new Push { DestinationRef = xTokenAddress });
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

            Core.vStack.Push(4, typeof(uint));
        }
    }
}
