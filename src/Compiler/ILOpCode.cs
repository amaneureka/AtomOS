/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
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
