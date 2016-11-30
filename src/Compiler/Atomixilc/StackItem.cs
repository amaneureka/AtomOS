using System;
using System.Collections.Generic;

using Atomixilc.Machine;

namespace Atomixilc
{
    internal class StackItem
    {
        internal Register? RegisterRef;
        internal string AddressRef;
        internal bool IsIndirect;
        internal int Displacement;
        internal Type OperandType;

        internal bool RegisterOnly
        {
            get { return (RegisterRef.HasValue && !IsIndirect); }
        }

        internal bool IsFloat
        {
            get
            {
                return ((OperandType == typeof(float))
                    || (OperandType == typeof(decimal))
                    || (OperandType == typeof(double)));
            }
        }

        internal bool IsInteger
        {
            get
            {
                return ((OperandType == typeof(char))
                    || (OperandType == typeof(int))
                    || (OperandType == typeof(uint))
                    || (OperandType == typeof(long))
                    || (OperandType == typeof(ulong))
                    || (OperandType == typeof(byte))
                    || (OperandType == typeof(sbyte))
                    || (OperandType == typeof(short))
                    || (OperandType == typeof(ushort)));
            }
        }
    }
}
