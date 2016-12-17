/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Newobj MSIL
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
            var functionInfo = ((OpMethod)xOp).Value;

            var addressRefernce = functionInfo.FullName();
            var parameters = functionInfo.GetParameters();
            var type = functionInfo.DeclaringType;
            var memsize = Helper.GetStorageSize(type, Config.TargetPlatform);
            var paramsize = parameters.Sum(para => Helper.GetTypeSize(para.ParameterType, Config.TargetPlatform, true));

            int count = parameters.Length;
            if (Optimizer.vStack.Count < count)
                throw new Exception("Internal Compiler Error: vStack.Count < expected size");

            /* The stack transitional behavior, in sequential order, is:
             * Arguments arg1 through argn are pushed on the stack in sequence.
             * Arguments argn through arg1 are popped from the stack and passed to ctor for object creation.
             * A reference to the new object is pushed onto the stack.
             */

            new Comment(string.Format("[{0}] : {1} => {2}", ToString(), xOp.ToString(), Optimizer.vStack.Count));

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
                            if (parameters.Length == 1 && parameters[0].ParameterType.ToString() == "System.Char[]")
                            {
                                new Mov { DestinationReg = Register.EAX, SourceReg = Register.ESP, SourceIndirect = true };
                                new Mov { DestinationReg = Register.EAX, SourceReg = Register.EAX, SourceIndirect = true, SourceDisplacement = 8 };
                                new Shl { DestinationReg = Register.EAX, SourceRef = "0x1" };
                            }
                            else if (parameters.Length == 1 && parameters[0].ParameterType.ToString() == "System.Char*")
                            {
                                new Push { DestinationReg = Register.ESP, DestinationIndirect = true };
                                new Call { DestinationRef = "getLength_System_Char__", IsLabel = true };
                                new Shl { DestinationReg = Register.EAX, SourceRef = "0x1" };
                            }
                            else if (parameters.Length == 3 && parameters[0].ParameterType.ToString() == "System.Char[]" && parameters[1].ParameterType.ToString() == "System.Int32" && parameters[2].ParameterType.ToString() == "System.Int32")
                            {
                                new Mov { DestinationReg = Register.EAX, SourceReg = Register.ESP, SourceIndirect = true };
                                new Shl { DestinationReg = Register.EAX, SourceRef = "0x1" };
                            }
                            else if (parameters.Length == 2 && parameters[0].ParameterType.ToString() == "System.Char" && parameters[1].ParameterType.ToString() == "System.Int32")
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
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }
        }
    }
}
