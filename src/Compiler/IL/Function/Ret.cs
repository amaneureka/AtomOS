/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Ret MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System.Reflection;

using Atomix.Assembler.x86;
using Atomix.CompilerExt;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.IL
{
    [ILOp(ILCode.Ret)]
    public class ILRet : MSIL
    {
        public ILRet(Compiler Cmp)
            : base("ret", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            //Current method end label without exception
            var xTargetLabel = aMethod.FullName() + ".End";

            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        //Just jump to End of label of current method as simple as that
                        Core.AssemblerCode.Add(new Jmp { DestinationRef = xTargetLabel });
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
    }
}
