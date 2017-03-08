/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          MSIL Opcode
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System.Reflection;

namespace Atomix
{
    public abstract class ILOpCode
    {
        public readonly ILCode Code;
        public readonly int Position;
        public readonly int NextPosition;
        public readonly ExceptionHandlingClause Ehc;

        public ILOpCode(ILCode aCode, int aPosition, int aNextPosition, ExceptionHandlingClause aEhc)
        {
            Code = aCode;
            Position = aPosition;
            NextPosition = aNextPosition;
            Ehc = aEhc;
        }
    }
}
