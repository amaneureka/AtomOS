/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Ldsfld MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;
using System.Reflection;
using System.Linq;

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

            string cctor_addref = null;

            var cctor = (field.DeclaringType.GetConstructors(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public) ?? new ConstructorInfo[0]).SingleOrDefault();
            if (cctor != null)
                cctor_addref = cctor.FullName();

            /* The stack transitional behavior, in sequential order, is:
             * The value of the specific field is pushed onto the stack.
             */

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        if (!string.IsNullOrEmpty(cctor_addref) && cctor != method)
                            new Call { DestinationRef = cctor_addref };

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
                        Optimizer.SaveStack(xOp.NextPosition);
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }
        }
    }
}
