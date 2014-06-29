using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Atomix;

namespace Atomix.ILOpCodes
{
    public class OpInt64 : ILOpCode
    {
        public readonly UInt64 Value;

        public OpInt64(ILCode c, int pos, int np, UInt64 aValue, ExceptionHandlingClause ehc)
            :base (c, pos, np, ehc)
        {
            Value = aValue;
        }

        public override string ToString()
        {
            return string.Format("ILOpCode=>Int64       [0x{1}-0x{2}] {0}:     {3}", Code, Position.ToString("X").PadLeft(3, '0'), NextPosition.ToString("X").PadLeft(3, '0'), Value);
        }
    }
}
