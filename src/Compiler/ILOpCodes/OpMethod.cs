/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          MSIL OpMethod
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System.Collections.Generic;
using System.Reflection;

namespace Atomix.ILOpCodes
{
    public class OpMethod : ILOpCode
    {
        public readonly MethodBase Value;
        public readonly uint MethodUID;

        private static uint Counter = 1;
        //public static Dictionary<MethodBase, uint> MethodUIDs = new Dictionary<MethodBase, uint>();

        public OpMethod(ILCode aCode, int aPosition, int aNextPosition, MethodBase aValue, ExceptionHandlingClause aEhc)
            : base(aCode, aPosition, aNextPosition, aEhc)
        {
            Value = aValue;
            MethodUID = 0;

            if (aValue.IsAbstract)
            {
                MethodUID = (uint)aValue.GetHashCode();
                /*
                if (MethodUIDs.ContainsKey(aValue))
                {
                    MethodUID = MethodUIDs[aValue];
                }
                else
                {
                    MethodUID = Counter++;
                    MethodUIDs.Add(aValue, MethodUID);
                }*/
            }
        }

        public override string ToString()
        {
            return string.Format("ILOpCode=>Method   [0x{1}-0x{2}] {0}:     {3}", Code, Position.ToString("X").PadLeft(3, '0'), NextPosition.ToString("X").PadLeft(3, '0'), Value);
        }
    }
}
