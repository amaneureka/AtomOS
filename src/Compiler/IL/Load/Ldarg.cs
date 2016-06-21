/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
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
            var xDisplacement = GetArgumentDisplacement(aMethod, aParam);

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

        public static int GetArgumentDisplacement(MethodBase aMethod, ushort aParam)
        {
            var xMethodBase = aMethod;
            
            var xMethodInfo = xMethodBase as System.Reflection.MethodInfo;
            int xReturnSize = 0;

            if (xMethodInfo != null)
                xReturnSize = xMethodInfo.ReturnType.SizeOf().Align();

            int xOffset = 8;
            var xCorrectedOpValValue = aParam;

            if (!aMethod.IsStatic && aParam > 0)
                xCorrectedOpValValue -= 1;

            var xParams = xMethodBase.GetParameters();
            if (aParam == 0 && !xMethodBase.IsStatic)
            {
                int xCurArgSize;
                if (xMethodBase.DeclaringType.IsValueType)
                    xCurArgSize = 4;
                else
                {
                    xCurArgSize = xMethodBase.DeclaringType.SizeOf().Align();
                }

                for (int i = xParams.Length - 1; i >= aParam; i--)
                {
                    var xSize = xParams[i].ParameterType.SizeOf().Align();
                    xOffset += xSize;
                }

                if (xReturnSize > xCurArgSize)
                {
                    int xExtraSize = xReturnSize - xCurArgSize;
                    xOffset += xExtraSize;
                }

                return (int)(xOffset + xCurArgSize - 4);
            }
            else
            {
                for (int i = xParams.Length - 1; i > xCorrectedOpValValue; i--)
                {
                    var xSize = xParams[i].ParameterType.SizeOf().Align();
                    xOffset += xSize;
                }

                var xCurArgSize = xParams[xCorrectedOpValValue].ParameterType.SizeOf().Align();
                int xArgSize = 0;
                foreach (var xParam in xParams)
                {
                    xArgSize += xParam.ParameterType.SizeOf().Align();
                }
                xReturnSize = 0;

                if (xReturnSize > xArgSize)
                {
                    int xExtraSize = xReturnSize - xArgSize;
                    xOffset += xExtraSize;
                }

                return (int)(xOffset + xCurArgSize - 4);
            }
        }
    }
}
