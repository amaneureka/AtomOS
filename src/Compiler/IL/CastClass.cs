/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
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
