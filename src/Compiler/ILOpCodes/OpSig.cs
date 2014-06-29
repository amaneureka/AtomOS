using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Atomix;

namespace Atomix.ILOpCodes
{
    public class OpSig : ILOpCode
    {
        public readonly Int32 Value;

        public OpSig(ILCode c, int pos, int np, Int32 aValue, ExceptionHandlingClause ehc)
            :base (c, pos, np, ehc)
        {
            Value = aValue;
        }

        public override string ToString()
        {
            return string.Format("ILOpCode=>Sig     [0x{1}-0x{2}] {0}:     {3}", Code, Position.ToString("X").PadLeft(3, '0'), NextPosition.ToString("X").PadLeft(3, '0'), Value);
        }
    }
}
