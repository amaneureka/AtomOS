/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Callvirt MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;
using System.Reflection;
using System.Linq;

using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.IL.CodeType;
using Atomixilc.Machine.x86;

namespace Atomixilc.IL
{
    [ILImpl(ILCode.Callvirt)]
    internal class Callvirt_il : MSIL
    {
        public Callvirt_il()
            : base(ILCode.Callvirt)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Callvirt(v=vs.110).aspx
         * Description : Calls a late-bound method on an object, pushing the return value onto the evaluation stack.
         */
        internal override void Execute(Options Config, OpCodeType xOp, MethodBase method, Optimizer Optimizer)
        {
            var xOpMethod = (OpMethod)xOp;
            var functionInfo = xOpMethod.Value as MethodInfo;

            var addressRefernce = functionInfo.FullName();
            var parameters = functionInfo.GetParameters();

            var size = parameters.Sum(a => Helper.GetTypeSize(a.ParameterType, Config.TargetPlatform, true));

            int count = parameters.Length;
            if (!functionInfo.IsStatic)
                count++;

            if (Optimizer.vStack.Count < count)
                throw new Exception("Internal Compiler Error: vStack.Count < expected size");

            /* The stack transitional behavior, in sequential order, is:
             * An object reference obj is pushed onto the stack.
             * Method arguments arg1 through argN are pushed onto the stack.
             * Method arguments arg1 through argN and the object reference obj are popped from the stack;
             * the method call is performed with these arguments and control is transferred to the method
             * in obj referred to by the method metadata token. When complete, a return value is generated
             * by the callee method and sent to the caller.
             * The return value is pushed onto the stack.
             */

            new Comment(string.Format("[{0}] : {1} => {2}", ToString(), xOp.ToString(), Optimizer.vStack.Count));

            while (count > 0)
            {
                Optimizer.vStack.Pop();
                count--;
            }

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        if (Helper.GetTypeSize(functionInfo.ReturnType, Config.TargetPlatform) > 4)
                            throw new Exception(string.Format("UnImplemented '{0}'", msIL));

                        if (functionInfo.IsStatic || functionInfo.IsFinal || !functionInfo.IsVirtual || !functionInfo.IsAbstract)
                        {
                            new Call { DestinationRef = addressRefernce };
                            new Test { DestinationReg = Register.ECX, SourceRef = "0xFFFFFFFF" };
                            new Jmp { Condition = ConditionalJump.JNZ, DestinationRef = xOp.HandlerRef };
                        }
                        else
                        {
                            new Mov { DestinationReg = Register.EAX, SourceReg = Register.ESP, SourceDisplacement = size, SourceIndirect = true };
                            new Mov { DestinationReg = Register.EAX, SourceReg = Register.EAX, SourceIndirect = true };

                            new Push { DestinationRef = Helper.VTable_Flush };
                            new Push { DestinationRef = "0x" + xOpMethod.MethodUID.ToString("X") };
                            new Push { DestinationReg = Register.EAX };
                            new Call { DestinationRef = Helper.VTable_Label, IsLabel = true };
                            new Test { DestinationReg = Register.ECX, SourceRef = "0xFFFFFFFF" };
                            new Jmp { Condition = ConditionalJump.JNZ, DestinationRef = xOp.HandlerRef };

                            if (functionInfo.DeclaringType == typeof(object))
                                throw new Exception("Callvirt Object Declaring type not implemented");

                            new Call { DestinationRef = "EAX" };
                            new Test { DestinationReg = Register.ECX, SourceRef = "0xFFFFFFFF" };
                            new Jmp { Condition = ConditionalJump.JNZ, DestinationRef = xOp.HandlerRef };
                        }

                        if (functionInfo.ReturnType != typeof(void))
                        {
                            new Push { DestinationReg = Register.EAX };
                            Optimizer.vStack.Push(new StackItem(functionInfo.ReturnType));
                        }
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }
        }
    }
}
