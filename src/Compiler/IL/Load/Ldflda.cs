/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Ldflda MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System.Reflection;

using Atomix.Assembler;
using Atomix.Assembler.x86;
using Atomix.CompilerExt;
using Atomix.ILOpCodes;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.IL
{
    [ILOp(ILCode.Ldflda)]
    public class Ldflda : MSIL
    {
        public Ldflda(Compiler Cmp)
            : base("ldflda", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            /* Pop the object first */
            var xStackValue = Core.vStack.Pop();

            var xF = ((OpField)instr).Value;
            var aDeclaringType = xF.DeclaringType;
            var xFieldId = xF.FullName();

            FieldInfo xFieldInfo = null;

            //Now we have to calculate the offset of object, and also give us that field
            int xOffset = ILHelper.GetFieldOffset(aDeclaringType, xFieldId, out xFieldInfo);
            bool xNeedsGC = aDeclaringType.IsClass && !aDeclaringType.IsValueType;
            if (xNeedsGC)
                xOffset += 12; //Extra offset =)

            //As we are sure xFieldInfo should contain value as if not than it throws error in GetFieldOffset
            var xSize = xFieldInfo.FieldType.SizeOf();

            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        Core.AssemblerCode.Add(new Add { DestinationReg = Registers.ESP, DestinationIndirect = true, SourceRef = "0x" + xOffset.ToString("X") });
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
            Core.vStack.Push(4, xF.FieldType);
        }
    }
}
