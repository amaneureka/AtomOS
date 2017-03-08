/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          MSIL OpNone
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System.Reflection;

namespace Atomix.ILOpCodes
{
    public class OpNone : ILOpCode
    {
        public OpNone(ILCode aCode, int aPosition, int aNextPosition, ExceptionHandlingClause aEhc)
            : base(aCode, aPosition, aNextPosition, aEhc)
        {
        }

        public override string ToString()
        {
            return string.Format("ILOpCode=>None    [0x{1}-0x{2}] {0} {3}", Code, Position.ToString("X").PadLeft(3, '0'), NextPosition.ToString("X").PadLeft(3, '0'), Ehc != null? Ehc.HandlerOffset : 0);
        }
    }
}
