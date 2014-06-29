using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Atomix;

namespace Atomix.ILOpCodes
{
    public class OpNone : ILOpCode
    {
        public OpNone(ILCode c, int pos, int np, ExceptionHandlingClause ehc)
            :base (c, pos, np, ehc)
        {
        }

        public override string ToString()
        {
            return string.Format("ILOpCode=>None    [0x{1}-0x{2}] {0} {3}", Code, Position.ToString("X").PadLeft(3, '0'), NextPosition.ToString("X").PadLeft(3, '0'), Ehc != null? Ehc.HandlerOffset : 0);
        }
    }
}
