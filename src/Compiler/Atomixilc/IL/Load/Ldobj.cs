/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Ldobj MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;
using System.Reflection;

using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;
using Atomixilc.IL.CodeType;

namespace Atomixilc.IL
{
    [ILImpl(ILCode.Ldobj)]
    internal class Ldobj_il : MSIL
    {
        public Ldobj_il()
            : base(ILCode.Ldobj)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Ldobj(v=vs.110).aspx
         * Description : Copies the value type object pointed to by an address to the top of the evaluation stack.
         */
        internal override void Execute(Options Config, OpCodeType xOp, MethodBase method, Optimizer Optimizer)
        {
            if (Optimizer.vStack.Count < 1)
                throw new Exception("Internal Compiler Error: vStack.Count < 1");

            var xOpType = ((OpType)xOp).Value;
            var size = Helper.GetTypeSize(xOpType, Config.TargetPlatform);

            /* The stack transitional behavior, in sequential order, is:
             * The address of a value type object is pushed onto the stack.
             * The address is popped from the stack and the instance at that particular address is looked up.
             * The value of the object stored at that address is pushed onto the stack.
             */

            var item = Optimizer.vStack.Pop();

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        if (!item.SystemStack)
                            throw new Exception(string.Format("UnImplemented-RegisterType '{0}'", msIL));

                        new Pop { DestinationReg = Register.EAX };
                        for (int i = 1; i <= (size / 4); i++)
                        {
                            new Push
                            {
                                DestinationReg = Register.EAX,
                                DestinationIndirect = true,
                                DestinationDisplacement = (size - (i * 4))
                            };
                        }

                        switch (size % 4)
                        {
                            case 0: break;
                            case 1:
                                {
                                    new Movzx { DestinationReg = Register.EAX, SourceReg = Register.ESI, SourceIndirect = true, Size = 8 };
                                    new Push { DestinationReg = Register.EAX };
                                }
                                break;
                            case 2:
                                {
                                    new Movzx { DestinationReg = Register.EAX, SourceReg = Register.ESI, SourceIndirect = true, Size = 16 };
                                    new Push { DestinationReg = Register.EAX };
                                }
                                break;
                            default:
                                throw new Exception("Unsupported Size");
                        }

                        Optimizer.vStack.Push(new StackItem(xOpType));
                        Optimizer.SaveStack(xOp.NextPosition);
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }
        }
    }
}
