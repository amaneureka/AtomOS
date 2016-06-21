/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          CastClass MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/


using System.Reflection;

namespace Atomix.IL
{
    [ILOp(ILCode.Castclass)]
    public class CastClass : MSIL
    {
        public CastClass(Compiler Cmp)
            : base("CastClass", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            //Do Nothing
            //COMPILER HACK ^^
        }
    }
}
