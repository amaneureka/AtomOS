﻿using System;
using System.Collections.Generic;
using System.Reflection;

using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;
using Atomixilc.IL.CodeType;

namespace Atomixilc.IL
{
    [ILImpl(ILCode.Ldloc)]
    internal class Ldloc_il : MSIL
    {
        public Ldloc_il()
            : base(ILCode.Ldloc)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Ldloc(v=vs.110).aspx
         * Description : Loads the local variable at a specific index onto the evaluation stack.
         */
        internal override void Execute(Options Config, OpCodeType xOp, MethodBase method, Optimizer Optimizer)
        {
            var index = ((OpVar)xOp).Value;

            var body = method.GetMethodBody();
            var EBPoffset = Helper.GetVariableOffset(body, index, Config.TargetPlatform);

            var varType = body.LocalVariables[index].LocalType;
            var size = Helper.GetTypeSize(varType, Config.TargetPlatform);

            /* The stack transitional behavior, in sequential order, is:
             * The local variable value at the specified index is pushed onto the stack.
             */

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        if (size > 4)
                            throw new Exception("LocalVariable size > 4 not supported");

                        switch(size)
                        {
                            case 1:
                            case 2:
                                {
                                    if (Helper.IsSigned(varType))
                                    {
                                        new Movsx
                                        {
                                            DestinationReg = Register.EAX,
                                            SourceReg = Register.EBP,
                                            SourceIndirect = true,
                                            SourceDisplacement = -EBPoffset,
                                            Size = (byte)(size * 8)
                                        };
                                    }
                                    else
                                    {
                                        new Movzx
                                        {
                                            DestinationReg = Register.EAX,
                                            SourceReg = Register.EBP,
                                            SourceIndirect = true,
                                            SourceDisplacement = -EBPoffset,
                                            Size = (byte)(size * 8)
                                        };
                                    }

                                    new Push { DestinationReg = Register.EAX };
                                }
                                break;
                            case 4:
                                {
                                    new Push { DestinationReg = Register.EBP, DestinationIndirect = true, DestinationDisplacement = -EBPoffset };
                                }
                                break;
                        }
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }

            Optimizer.vStack.Push(new StackItem(varType));
        }
    }
}
