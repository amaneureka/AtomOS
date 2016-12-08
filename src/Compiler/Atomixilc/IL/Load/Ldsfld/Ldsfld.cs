using System;
using System.Collections.Generic;
using System.Reflection;

using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;
using Atomixilc.IL.CodeType;

namespace Atomixilc.IL
{
    [ILImpl(ILCode.Ldsfld)]
    internal class Ldsfld_il : MSIL
    {
        public Ldsfld_il()
            : base(ILCode.Ldsfld)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Ldsfld(v=vs.110).aspx
         * Description : Pushes the value of a static field onto the evaluation stack.
         */
        internal override void Execute(Options Config, OpCodeType xOp, MethodBase method, Optimizer Optimizer)
        {
            var field = ((OpField)xOp).Value;
            var fieldName = field.FullName();
            var size = Helper.GetTypeSize(field.FieldType, Config.TargetPlatform);

            /* The stack transitional behavior, in sequential order, is:
             * The value of the specific field is pushed onto the stack.
             */

            new Comment(string.Format("[{0}] : {1} => {2}", ToString(), xOp.ToString(), Optimizer.vStack.Count));

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        for (int i = 1; i <= (size / 4); i++)
                        {
                            new Push
                            {
                                DestinationRef = fieldName,
                                DestinationIndirect = true,
                                DestinationDisplacement = (size - (i * 4))
                            };
                        }

                        switch (size % 4)
                        {
                            case 0: break;
                            case 1:
                                {
                                    new Movzx { DestinationReg = Register.EAX, SourceRef = fieldName, SourceIndirect = true, Size = 8 };
                                    new Push { DestinationReg = Register.EAX };
                                }
                                break;
                            case 2:
                                {
                                    new Movzx { DestinationReg = Register.EAX, SourceRef = fieldName, SourceIndirect = true, Size = 16 };
                                    new Push { DestinationReg = Register.EAX };
                                }
                                break;
                            default:
                                throw new Exception("Unsupported Size");
                        }

                        Optimizer.vStack.Push(new StackItem(field.FieldType));
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }
        }
    }
}
