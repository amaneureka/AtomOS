/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Stelem_Ref MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;
using System.Reflection;

using Atomix.Assembler;
using Atomix.Assembler.x86;
using Atomix.CompilerExt;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.IL
{
    [ILOp(ILCode.Stelem_Ref)]
    public class Stelem_Ref : MSIL
    {
        public Stelem_Ref(Compiler Cmp)
            : base("stelem_ref", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        Stelem_x86(this.Compiler, instr, aMethod, 4);
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

        public static void Stelem_x86(Compiler cmp, ILOpCode instr, MethodBase aMethod, int aElementSize)
        {
            int xStackSize = aElementSize.Align();

            Core.vStack.Pop();
            Core.vStack.Pop();
            Core.vStack.Pop();

            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.ESP, SourceIndirect = true, SourceDisplacement = (int)xStackSize }); // the index
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, SourceReg = Registers.ESP, SourceIndirect = true, SourceDisplacement = (int)xStackSize + 4 }); // the index

            Core.AssemblerCode.Add(new Add { DestinationReg = Registers.ECX, SourceRef = "0x10" });

            Core.AssemblerCode.Add(new Push { DestinationRef = "0x" + aElementSize.ToString("X") });
            Core.vStack.Push(4, typeof(int));

            Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EAX });
            Core.vStack.Push(4, typeof(int));

            cmp.MSIL[ILCode.Mul].Execute(instr, aMethod);

            Core.AssemblerCode.Add(new Push { DestinationReg = Registers.ECX });
            Core.vStack.Push(4, typeof(int));

            cmp.MSIL[ILCode.Add].Execute(instr, aMethod);

            Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
            Core.vStack.Pop();
            for (int i = (int)(aElementSize / 4) - 1; i >= 0; i -= 1)
            {
                Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EBX });
                Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, DestinationIndirect = true, SourceReg = Registers.EBX });
                Core.AssemblerCode.Add(new Add { DestinationReg = Registers.EAX, SourceRef = "0x4" });
            }
            switch (aElementSize % 4)
            {
                case 1:
                    {
                        Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EBX });
                        Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, DestinationIndirect = true, SourceReg = Registers.BL, Size = 8 });
                        break;
                    }
                case 2:
                    {
                        Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EBX });
                        Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, DestinationIndirect = true, SourceReg = Registers.BX, Size = 16 });
                        break;
                    }
                case 0:
                    {
                        break;
                    }
                default:
                    throw new Exception("@Stelem: Remainder size " + (aElementSize % 4) + " not supported!");

            }
            Core.AssemblerCode.Add(new Add { DestinationReg = Registers.ESP, SourceRef = "0x8" });
        }
    }
}
