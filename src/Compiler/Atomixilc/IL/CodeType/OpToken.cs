/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Token Value MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;
using System.Reflection;

namespace Atomixilc.IL.CodeType
{
    internal class OpToken : OpCodeType
    {
        internal readonly int Value;
        internal readonly Type ValueType;
        internal readonly FieldInfo ValueField;

        internal bool IsType
        {
            get
            {
                if (((Value & 0x02000000) != 0)
                    || ((Value & 0x01000000) != 0)
                    || ((Value & 0x1B000000) != 0))
                    return true;
                return false;
            }
        }

        internal bool IsField
        {
            get
            {
                return ((Value & 0x04000000) != 0);
            }
        }

        internal OpToken(ILCode aCode, int aPosition, int aNextPosition, int aValue, Module aModule, Type[] aTypeGenericArgs, Type[] aMethodGenericArgs, ExceptionHandlingClause aEhc)
            :base(aCode, aPosition, aNextPosition, aEhc)
        {
            Value = aValue;
            if (IsField)
                ValueField = aModule.ResolveField(Value, aTypeGenericArgs, aMethodGenericArgs);
            if (IsType)
                ValueType = aModule.ResolveType(Value, aTypeGenericArgs, aMethodGenericArgs);
        }

        public override string ToString()
        {
            return string.Format("{0} [0x{1}-0x{2}] {3}", GetType().Name, Position.ToString("X4"), NextPosition.ToString("X4"), IsField ? ValueField.ToString() : ValueType.ToString());
        }
    }
}
