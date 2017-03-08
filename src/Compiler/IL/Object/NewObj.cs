/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          NewObj MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;
using System.Linq;
using System.Reflection;

using Atomix.Assembler;
using Atomix.Assembler.x86;
using Atomix.CompilerExt;
using Atomix.ILOpCodes;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.IL
{
    [ILOp(ILCode.Newobj)]
    public class NewObj : MSIL
    {
        public NewObj(Compiler Cmp)
            : base("newobj", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            var xOperand = ((OpMethod)instr);
            var xTargetMethod = xOperand.Value;
            var xTargetType = xTargetMethod.DeclaringType;
            var xCurrentLabel = ILHelper.GetLabel(aMethod, xOperand.Position);

            var xEndException = aMethod.FullName() + ".Error";
            if (instr.Ehc != null && instr.Ehc.HandlerOffset > instr.Position)
            {
                xEndException = ILHelper.GetLabel(aMethod, instr.Ehc.HandlerOffset);
            }

            string xCctorAddress = null;
            if (aMethod != null)
            {
                var xCctor = (xTargetType.GetConstructors(BindingFlags.Static | BindingFlags.NonPublic) ?? new ConstructorInfo[0]).SingleOrDefault();
                if (xCctor != null)
                    xCctorAddress = xCctor.FullName();
            }

            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        if (xCctorAddress != null)
                        {
                            Core.AssemblerCode.Add(new Call(xCctorAddress));
                            Core.AssemblerCode.Add(new Test { DestinationReg = Registers.ECX, SourceRef = "0x2" });
                            Core.AssemblerCode.Add(new Jmp { Condition = ConditionalJumpEnum.JNE, DestinationRef = xEndException });
                        }

                        if (xTargetType.IsValueType)
                        {
                            int xStorageSize = xTargetType.SizeOf().Align();
                            int xArgSize = 0;

                            var xParameterList = xTargetMethod.GetParameters();
                            foreach (var xParam in xParameterList)
                            {
                                xArgSize += xParam.ParameterType.SizeOf().Align();
                            }

                            int xShift = (int)(xArgSize - xStorageSize);
                            if (xShift < 0)
                            {
                                Core.AssemblerCode.Add(new Sub { DestinationReg = Registers.ESP, SourceRef = "0x" + Math.Abs(xShift).ToString("X") });
                            }
                            else if (xShift > 0)
                            {
                                Core.AssemblerCode.Add(new Add { DestinationReg = Registers.ESP, SourceRef = "0x" + xShift.ToString("X") });
                            }

                            //Create space for struc. pointer
                            Core.AssemblerCode.Add(new Push { DestinationReg = Registers.ESP });
                            Core.vStack.Push(4, typeof(IntPtr));

                            foreach (var xParam in xParameterList)
                            {
                                int xArgSizeForThis = xParam.ParameterType.SizeOf().Align();
                                Core.vStack.Push(xArgSizeForThis, xParam.ParameterType);
                                for (int i = 1; i <= xArgSizeForThis / 4; i++)
                                {
                                    Core.AssemblerCode.Add(new Push { DestinationReg = Registers.ESP, DestinationIndirect = true, DestinationDisplacement = (int)xStorageSize });
                                }
                            }

                            new ILCall(this.Compiler).Execute(xOperand, aMethod);
                            Core.vStack.Push(xStorageSize, xTargetType);
                        }
                        else
                        {
                            var xParams = xTargetMethod.GetParameters();
                            for (int i = 0; i < xParams.Length; i++)
                            {
                                Core.vStack.Pop();
                            }

                            bool xHasCalcSize = false;

                            if (xTargetType.ToString() == "System.String")
                            {
                                xHasCalcSize = true;

                                if (xParams.Length == 1 && xParams[0].ParameterType.ToString() == "System.Char[]")
                                {
                                    Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.ESP, SourceIndirect = true });
                                    Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EAX, SourceIndirect = true, SourceDisplacement = 8 });
                                    Core.AssemblerCode.Add(new ShiftLeft { DestinationReg = Registers.EAX, SourceRef = "0x1" });
                                    Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EAX });
                                }
                                else if (xParams.Length == 1 && xParams[0].ParameterType.ToString() == "System.Char*")
                                {
                                    Core.AssemblerCode.Add(new Push { DestinationReg = Registers.ESP, DestinationIndirect = true });
                                    Core.AssemblerCode.Add(new Call("getLength_System_Char__", true));
                                    Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                                    Core.AssemblerCode.Add(new ShiftLeft { DestinationReg = Registers.EAX, SourceRef = "0x1" });
                                    Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EAX });
                                }
                                else if (xParams.Length == 3 && xParams[0].ParameterType.ToString() == "System.Char[]" && xParams[1].ParameterType.ToString() == "System.Int32" && xParams[2].ParameterType.ToString() == "System.Int32")
                                {
                                    Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.ESP, SourceIndirect = true });
                                    Core.AssemblerCode.Add(new ShiftLeft { DestinationReg = Registers.EAX, SourceRef = "0x1" });
                                    Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EAX });
                                }
                                else if (xParams.Length == 2 && xParams[0].ParameterType.ToString() == "System.Char" && xParams[1].ParameterType.ToString() == "System.Int32")
                                {
                                    Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.ESP, SourceIndirect = true });
                                    Core.AssemblerCode.Add(new ShiftLeft { DestinationReg = Registers.EAX, SourceRef = "0x1" });
                                    Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EAX });
                                }
                                else
                                    throw new NotImplementedException("In NewObj is a string ctor implementation missing!`" + xParams[0].ParameterType.ToString() + "`");
                            }

                            int xMemSize = ILHelper.StorageSize(xTargetType) + 12;
                            Core.AssemblerCode.Add(new Push { DestinationRef = "0x" + xMemSize.ToString("X") });
                            if (xHasCalcSize)
                            {
                                Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                                Core.AssemblerCode.Add(new Add { DestinationReg = Registers.ESP, DestinationIndirect = true, SourceReg = Registers.EAX });
                            }

                            //Call our Heap
                            Core.AssemblerCode.Add(new Call(Helper.lblHeap, true));
                            Core.AssemblerCode.Add(new Push { DestinationReg = Registers.ESP, DestinationIndirect = true });
                            Core.AssemblerCode.Add(new Push { DestinationReg = Registers.ESP, DestinationIndirect = true });

                            int xGCFieldCount = xTargetType.GetFields().Count(x => x.FieldType.IsValueType);

                            var xTypeID = ILHelper.GetTypeID(xTargetType);

                            Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, DestinationIndirect = true, SourceRef = "0x" + xTypeID.ToString("X") });
                            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, DestinationIndirect = true, DestinationDisplacement = 4, SourceRef = "0x1" });
                            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, DestinationIndirect = true, DestinationDisplacement = 8, SourceRef = "0x" + xMemSize.ToString("X") });
                            uint xSize = (uint)(((from item in xParams
                                                  let xQSize = item.ParameterType.SizeOf().Align()
                                                  select (int)xQSize).Take(xParams.Length).Sum()));

                            foreach (var xParam in xParams)
                            {
                                int xParamSize = xParam.ParameterType.SizeOf().Align();
                                for (int i = 0; i < xParamSize; i += 4)
                                {
                                    Core.AssemblerCode.Add(new Push { DestinationReg = Registers.ESP, DestinationIndirect = true, DestinationDisplacement = (int)(xSize + 4) });
                                }
                            }

                            Core.AssemblerCode.Add(new Call(xTargetMethod.FullName()));
                            if (aMethod != null)
                            {
                                Core.AssemblerCode.Add(new Test { DestinationReg = Registers.ECX, SourceRef = "0x2" });
                                string xNoErrorLabel = xCurrentLabel + ".NoError";
                                Core.AssemblerCode.Add(new Jmp { Condition = ConditionalJumpEnum.JE, DestinationRef = xNoErrorLabel });

                                PushAlignedParameterSize(xTargetMethod);
                                // an exception occurred, we need to cleanup the stack, and jump to the exit
                                Core.AssemblerCode.Add(new Add { DestinationReg = Registers.ESP, SourceRef = "0x4" });

                                int xESPOffset = 0;
                                foreach (var xParam in xParams)
                                {
                                    xESPOffset += xParam.ParameterType.SizeOf().Align();
                                }
                                Core.AssemblerCode.Add(new Add { DestinationReg = Registers.ESP, SourceRef = "0x" + xESPOffset.ToString("X") });

                                Core.AssemblerCode.Add(new Jmp { DestinationRef = aMethod.FullName() + ".Error" });
                                Core.AssemblerCode.Add(new Label(xNoErrorLabel));
                            }
                            Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });

                            PushAlignedParameterSize(xTargetMethod);

                            Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EAX });
                            Core.vStack.Push(4, xTargetType);
                        }
                    }
                    break;
                #endregion
                #region _x64_
                case CPUArch.x64:
                    {

                    }
                    break;
                #endregion
                #region _ARM_
                case CPUArch.ARM:
                    {

                    }
                    break;
                #endregion
            }
        }
        private static void PushAlignedParameterSize(System.Reflection.MethodBase aMethod)
        {
            System.Reflection.ParameterInfo[] xParams = aMethod.GetParameters();

            int xSize = 0;
            for (int i = 0; i < xParams.Length; i++)
                xSize += xParams[i].ParameterType.SizeOf().Align();
            Core.AssemblerCode.Add(new Add { DestinationReg = Registers.ESP, SourceRef = "0x" + xSize.ToString("X") });
        }
    }
}
