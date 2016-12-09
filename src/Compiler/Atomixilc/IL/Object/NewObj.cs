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

            while (count > 0)
            {
                Optimizer.vStack.Pop();
                count--;
            }

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        if (type.IsValueType)
                            throw new Exception("Newobj ValueType not implemented");

                        if (type == typeof(string))
                            throw new Exception("string not supported");

                        new Push { DestinationRef = "0x" + memsize.ToString("X") };
                        new Call { DestinationRef = Helper.Heap_Label, IsLabel = true };

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
                        new Pop { DestinationReg = Register.EAX };
                        new Add { DestinationReg = Register.ESP, DestinationRef = "0x" + paramsize.ToString("X") };
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
