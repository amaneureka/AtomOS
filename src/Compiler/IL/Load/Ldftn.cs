/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Ldftn MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System.Reflection;

using Atomix.Assembler.x86;
using Atomix.CompilerExt;
using Atomix.ILOpCodes;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.IL
{
    [ILOp(ILCode.Ldftn)]
    public class Ldftn : MSIL
    {
        public Ldftn(Compiler Cmp)
            : base("ldftn", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            var xMethod = ((OpMethod)instr).Value;
            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        //Push 0x0 on stack
                        Core.AssemblerCode.Add(new Push { DestinationRef = xMethod.FullName() });
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
            Core.vStack.Push(4, typeof(uint));
        }
    }
}
