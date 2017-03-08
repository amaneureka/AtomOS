/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Ldarg MSIL
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
    [ILOp(ILCode.Ldarg)]
    public class Ldarg : MSIL
    {
        public Ldarg(Compiler Cmp)
            : base("ldarg", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            var aParam = ((OpVar)instr).Value;
            Execute2(aParam, aMethod);
        }

        public void Execute2(ushort aParam, MethodBase aMethod)
        {
            var xDisplacement = ILHelper.GetArgumentDisplacement(aMethod, aParam);

            Type xArgType;
            if (aMethod.IsStatic)
            {
                xArgType = aMethod.GetParameters()[aParam].ParameterType;
            }
            else
            {
                if (aParam == 0u)
                {
                    xArgType = aMethod.DeclaringType;
                    if (xArgType.IsValueType)
                    {
                        xArgType = xArgType.MakeByRefType();
                    }
                }
                else
                {
                    xArgType = aMethod.GetParameters()[aParam - 1].ParameterType;
                }
            }

            int xArgRealSize = xArgType.SizeOf();
            int xArgSize = xArgRealSize.Align();

            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        if (xArgRealSize < 4)
                        {
                            Core.AssemblerCode.Add(
                                new Movsx
                                {
                                    DestinationReg = Registers.EAX,
                                    Size = (byte)(xArgRealSize * 8),
                                    SourceReg = Registers.EBP,
                                    SourceIndirect = true,
                                    SourceDisplacement = xDisplacement
                                });
                            Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EAX });
                        }
                        else
                        {
                            for (int i = 0; i < (xArgSize / 4); i++)
                            {
                                Core.AssemblerCode.Add(
                                    new Push
                                    {
                                        DestinationReg = Registers.EBP,
                                        DestinationIndirect = true,
                                        DestinationDisplacement = xDisplacement - (i * 4)
                                    });
                            }
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

            Core.vStack.Push(xArgSize, xArgType);
        }
    }
}
