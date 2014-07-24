using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Atomix;

namespace Atomix.ILOpCodes
{
    public class OpMethod : ILOpCode
    {
        public readonly MethodBase Value;
        public readonly uint MethodUID;

        private static uint Counter = 1;
        public static Dictionary<MethodBase, uint> MethodUIDs = new Dictionary<MethodBase, uint>();

        public OpMethod(ILCode c, int pos, int np, MethodBase aValue, ExceptionHandlingClause ehc)
            :base (c, pos, np, ehc)
        {
            Value = aValue;
            MethodUID = 0;

            if (aValue.IsAbstract)
            {
                if (MethodUIDs.ContainsKey(aValue))
                {
                    MethodUID = MethodUIDs[aValue];
                }
                else
                {
                    MethodUID = Counter++;
                    MethodUIDs.Add(aValue, MethodUID);
                }
            }
        }

        public override string ToString()
        {
            return string.Format("ILOpCode=>Method   [0x{1}-0x{2}] {0}:     {3}", Code, Position.ToString("X").PadLeft(3, '0'), NextPosition.ToString("X").PadLeft(3, '0'), Value);
        }
    }
}
