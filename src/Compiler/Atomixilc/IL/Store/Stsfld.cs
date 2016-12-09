﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;
using Atomixilc.IL.CodeType;

namespace Atomixilc.IL
{
    [ILImpl(ILCode.Stsfld)]
    internal class Stsfld_il : MSIL
    {
        public Stsfld_il()
            : base(ILCode.Stsfld)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Stsfld(v=vs.110).aspx
         * Description : Replaces the value of a static field with a value from the evaluation stack.
         */
        internal override void Execute(Options Config, OpCodeType xOp, MethodBase method, Optimizer Optimizer)
        {
            if (Optimizer.vStack.Count < 1)
                throw new Exception("Internal Compiler Error: vStack.Count < 1");

            var field = ((OpField)xOp).Value;
            var fieldName = field.FullName();
            var size = Helper.GetTypeSize(field.FieldType, Config.TargetPlatform);

            string cctor_addref = null;

            var cctor = (field.DeclaringType.GetConstructors(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public) ?? new ConstructorInfo[0]).SingleOrDefault();
            if (cctor != null)
                cctor_addref = cctor.FullName();

            /* The stack transitional behavior, in sequential order, is:
             * A value is pushed onto the stack.
             * A value is popped from the stack and stored in field.
             */

            new Comment(string.Format("[{0}] : {1} => {2}", ToString(), xOp.ToString(), Optimizer.vStack.Count));

            var item = Optimizer.vStack.Pop();

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        if (!string.IsNullOrEmpty(cctor_addref) && cctor != method)
                            new Call { DestinationRef = cctor_addref };

                        for (int i = 0; i < size; i += 4)
                        {
                            new Pop { DestinationReg = Register.EAX };
                            new Mov { DestinationRef = fieldName, DestinationIndirect = true, DestinationDisplacement = i, SourceReg = Register.EAX };
                        }

                        int offset = size / 4;
                        switch (size % 4)
                        {
                            case 0: break;
                            case 1:
                                {
                                    new Pop { DestinationReg = Register.EAX };
                                    new Mov { DestinationRef = fieldName, DestinationIndirect = true, DestinationDisplacement = offset * 4, SourceReg = Register.EAX, Size = 8 };
                                }
                                break;
                            case 2:
                                {
                                    new Pop { DestinationReg = Register.EAX };
                                    new Mov { DestinationRef = fieldName, DestinationIndirect = true, DestinationDisplacement = offset * 4, SourceReg = Register.EAX, Size = 16 };
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
        }
    }
}
