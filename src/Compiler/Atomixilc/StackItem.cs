using System;

namespace Atomixilc
{
    internal class StackItem
    {
        internal readonly bool SystemStack;
        internal readonly Type OperandType;

        internal StackItem(Type aType)
        {
            SystemStack = true;
            OperandType = aType;
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

        public bool Equals(StackItem aItem)
        {
            return (SystemStack == aItem.SystemStack) &&
                   (OperandType == aItem.OperandType);
        }
    }
}
