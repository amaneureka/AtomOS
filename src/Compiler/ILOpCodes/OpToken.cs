/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          MSIL OpToken
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;
using System.Reflection;

namespace Atomix.ILOpCodes
{
    public class OpToken : ILOpCode
    {
        public readonly int Value;
        public readonly FieldInfo ValueField;
        public readonly Type ValueType;

        public bool ValueIsType
        {
            get
            {
                if ((Value & 0x02000000) != 0)
                {
                    return true;
                }
                if ((Value & 0x01000000) != 0)
                {
                    return true;
                }
                if ((Value & 0x1B000000) != 0)
                {
                    return true;
                }
                return false;
            }
        }
        public bool ValueIsField
        {
            get
            {
                if ((Value & 0x04000000) != 0)
                {
                    return true;
                }
                return false;
            }
        }

        public OpToken(ILCode aCode, int aPosition, int aNextPosition, int aValue, Module aModule, Type[] aTypeGenericArgs, Type[] aMethodGenericArgs, ExceptionHandlingClause aEhc)
            : base(aCode, aPosition, aNextPosition, aEhc)
        {
            Value = aValue;
            if (ValueIsField)
            {
                ValueField = aModule.ResolveField(Value, aTypeGenericArgs, aMethodGenericArgs);
            }
            if (ValueIsType)
            {
                ValueType = aModule.ResolveType(Value, aTypeGenericArgs, aMethodGenericArgs);
            }
        }

        public override string ToString()
        {
            return string.Format("ILOpCode=>Token    [0x{1}-0x{2}] {0}:     {3}", Code, Position.ToString("X").PadLeft(3, '0'), NextPosition.ToString("X").PadLeft(3, '0'), Value);
        }
    }
}
