using System;
using System.Collections.Generic;

using Atomixilc.Machine;

namespace Atomixilc
{
    internal class StackItem
    {
        internal readonly Register? RegisterRef;
        internal readonly string AddressRef;

        internal readonly bool IsIndirect;
        internal readonly bool SystemStack;

        internal readonly int Displacement;

        internal readonly Type OperandType;

        internal StackItem(Register aReg, Type aType, bool aIndirect = false, int aDisplacement = 0)
        {
            RegisterRef = aReg;
            OperandType = aType;
            IsIndirect = aIndirect;
            Displacement = aDisplacement;
        }

        internal StackItem(Type aType)
        {
            SystemStack = true;
            OperandType = aType;
        }

        internal StackItem(string aAddress, Type aType, bool aIndirect = false, int aDisplacement = 0)
        {
            AddressRef = aAddress;
            OperandType = aType;
            IsIndirect = aIndirect;
            Displacement = aDisplacement;
        }

        internal bool RegisterOnly
        {
            get { return (RegisterRef.HasValue && !IsIndirect); }
        }

        internal bool MemoryReference
        {
            get { return IsIndirect; }
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
            return (RegisterRef == aItem.RegisterRef) &&
                   (AddressRef == aItem.AddressRef) &&
                   (IsIndirect == aItem.IsIndirect) &&
                   (SystemStack == aItem.SystemStack) &&
                   (Displacement == aItem.Displacement) &&
                   (OperandType == aItem.OperandType);
        }
    }
}
