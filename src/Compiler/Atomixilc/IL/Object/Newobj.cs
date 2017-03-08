/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Newobj MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;
using System.Reflection;
using System.Linq;
using System.Runtime.InteropServices;

using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;
using Atomixilc.IL.CodeType;

namespace Atomixilc.IL
{
    [ILImpl(ILCode.Newobj)]
    internal class Newobj_il : MSIL
    {
        public Newobj_il()
            : base(ILCode.Newobj)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Newobj(v=vs.110).aspx
         * Description : Creates a new object or a new instance of a value type, pushing an object reference (type O) onto the evaluation stack.
         */
        internal override void Execute(Options Config, OpCodeType xOp, MethodBase method, Optimizer Optimizer)
        {
            var xOpMethod = (OpMethod)xOp;
            var functionInfo = xOpMethod.Value;

            var addressRefernce = functionInfo.FullName();
            var parameters = functionInfo.GetParameters();
            var type = functionInfo.DeclaringType;
            var memsize = Helper.GetStorageSize(type, Config.TargetPlatform);
            var paramsize = parameters.Sum(para => Helper.GetTypeSize(para.ParameterType, Config.TargetPlatform, true));

            int count = parameters.Length;
            if (Optimizer.vStack.Count < count)
                throw new Exception("Internal Compiler Error: vStack.Count < expected size");

            if (xOpMethod.CallingConvention != CallingConvention.StdCall)
                throw new Exception(string.Format("CallingConvention '{0}' not supported", xOpMethod.CallingConvention));

            /* The stack transitional behavior, in sequential order, is:
             * Arguments arg1 through argn are pushed on the stack in sequence.
             * Arguments argn through arg1 are popped from the stack and passed to ctor for object creation.
             * A reference to the new object is pushed onto the stack.
             */

            int index = count;
            while (index > 0)
            {
                Optimizer.vStack.Pop();
                index--;
            }

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        if (type.IsValueType)
                            throw new Exception("Newobj ValueType not implemented");

                        if (type == typeof(string))
                        {
                            if (parameters.Length == 1)
                            {
                                var param = parameters[0].ParameterType.ToString();
                                if (param == "System.Char[]")
                                {
                                    new Mov { DestinationReg = Register.EAX, SourceReg = Register.ESP, SourceIndirect = true };
                                    new Mov { DestinationReg = Register.EAX, SourceReg = Register.EAX, SourceIndirect = true, SourceDisplacement = 8 };
                                    new Shl { DestinationReg = Register.EAX, SourceRef = "0x1" };
                                }
                                else if (param == "System.Char*")
                                {
                                    new Push { DestinationReg = Register.ESP, DestinationIndirect = true };
                                    new Call { DestinationRef = "GetLength.System.Char*", IsLabel = true };
                                    new Shl { DestinationReg = Register.EAX, SourceRef = "0x1" };
                                }
                                else if (param == "System.SByte*")
                                {
                                    new Push { DestinationReg = Register.ESP, DestinationIndirect = true };
                                    new Call { DestinationRef = "GetLength.System.SByte*", IsLabel = true };
                                    new Shl { DestinationReg = Register.EAX, SourceRef = "0x1" };
                                }
                                else
                                    throw new Exception("String constructor not implemented");
                            }
                            else if (parameters.Length == 3)
                            {
                                new Mov { DestinationReg = Register.EAX, SourceReg = Register.ESP, SourceIndirect = true };
                                new Shl { DestinationReg = Register.EAX, SourceRef = "0x1" };
                            }
                            else if (parameters.Length == 2)
                            {
                                new Mov { DestinationReg = Register.EAX, SourceReg = Register.ESP, SourceIndirect = true };
                                new Shl { DestinationReg = Register.EAX, SourceRef = "0x1" };
                            }
                            else
                                throw new Exception("String constructor not supported");

                            new Add { DestinationReg = Register.EAX, SourceRef = "0x" + memsize.ToString("X") };
                            new Push { DestinationReg = Register.EAX };
                        }
                        else
                            new Push { DestinationRef = "0x" + memsize.ToString("X") };

                        new Call { DestinationRef = Helper.Heap_Label, IsLabel = true };
                        new Test { DestinationReg = Register.ECX, SourceRef = "0xFFFFFFFF" };
                        new Jmp { Condition = ConditionalJump.JNZ, DestinationRef = xOp.HandlerRef };

                        new Mov { DestinationReg = Register.EAX, DestinationIndirect = true, SourceRef = "0x" + type.GetHashCode().ToString("X") };
                        new Mov { DestinationReg = Register.EAX, DestinationIndirect = true, DestinationDisplacement = 4, SourceRef = "0x1" };
                        new Mov { DestinationReg = Register.EAX, DestinationIndirect = true, DestinationDisplacement = 8, SourceRef = "0x" + memsize.ToString("X") };

                        new Push { DestinationReg = Register.EAX };
                        new Push { DestinationReg = Register.EAX };

                        for (int i = 0; i < count; i++)
                        {
                            if (Helper.GetTypeSize(parameters[i].ParameterType, Config.TargetPlatform) > 4)
                                throw new Exception("unsupported size");
                            new Push { DestinationReg = Register.ESP, DestinationDisplacement = (paramsize + 4), DestinationIndirect = true };
                        }

                        new Call { DestinationRef = functionInfo.FullName() };
                        new Test { DestinationReg = Register.ECX, SourceRef = "0xFFFFFFFF" };
                        new Jmp { Condition = ConditionalJump.JNZ, DestinationRef = xOp.HandlerRef };

                        new Pop { DestinationReg = Register.EAX };
                        new Add { DestinationReg = Register.ESP, SourceRef = "0x" + paramsize.ToString("X") };
                        new Push { DestinationReg = Register.EAX };

                        Optimizer.vStack.Push(new StackItem(type));
                        Optimizer.SaveStack(xOp.NextPosition);
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }
        }
    }
}
