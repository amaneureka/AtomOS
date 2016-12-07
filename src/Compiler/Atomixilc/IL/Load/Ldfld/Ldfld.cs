﻿using System;
using System.Collections.Generic;
using System.Reflection;

using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;
using Atomixilc.IL.CodeType;

namespace Atomixilc.IL
{
    [ILImpl(ILCode.Ldfld)]
    internal class Ldfld_il : MSIL
    {
        public Ldfld_il()
            : base(ILCode.Ldfld)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Ldfld(v=vs.110).aspx
         * Description : Finds the value of a field in the object whose reference is currently on the evaluation stack
         */
        internal override void Execute(Options Config, OpCodeType xOp, MethodBase method, Optimizer Optimizer)
        {
            if (Optimizer.vStack.Count < 1)
                throw new Exception("Internal Compiler Error: vStack.Count < 1");

            var field = ((OpField)xOp).Value;
            var offset = Helper.GetFieldOffset(field.DeclaringType, field, Config.TargetPlatform);
            var size = Helper.GetTypeSize(field.FieldType, Config.TargetPlatform);

            /* The stack transitional behavior, in sequential order, is:
             * An object reference (or pointer) is pushed onto the stack.
             * The object reference (or pointer) is popped from the stack; the value of the specified field in the object is found.
             * The value stored in the field is pushed onto the stack.
             */

            new Comment(string.Format("[{0}] : {1}", ToString(), xOp.ToString()));

            Optimizer.vStack.Pop();

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        new Pop { DestinationReg = Register.EDX };
                        new Add { DestinationReg = Register.EDX, SourceRef = "0x" + offset.ToString("X") };

                        for (int i = 1; i <= (size / 4); i++)
                        {
                            new Mov { DestinationReg = Register.EAX, SourceReg = Register.EDX, SourceIndirect = true, SourceDisplacement = (size - (i * 4) + offset) };
                            new Push { DestinationReg = Register.EAX };
                        }

                        switch(size % 4)
                        {
                            case 1:
                                {
                                    new Movzx { DestinationReg = Register.EAX, SourceReg = Register.EDX, SourceIndirect = true, SourceDisplacement = offset, Size = 8 };
                                    new Push { DestinationReg = Register.EAX };
                                }
                                break;
                            case 2:
                                {
                                    new Movzx { DestinationReg = Register.EAX, SourceReg = Register.EDX, SourceIndirect = true, SourceDisplacement = offset, Size = 16 };
                                    new Push { DestinationReg = Register.EAX };
                                }
                                break;
                            default:
                                throw new Exception("Unsupported Size");
                        }
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }

            Optimizer.vStack.Push(new StackItem(field.FieldType));
        }
    }
}
