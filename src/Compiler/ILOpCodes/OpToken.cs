using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Atomix;

namespace Atomix.ILOpCodes
{
    public class OpToken : ILOpCode
    {
        public readonly Int32 Value;
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

        public OpToken(ILCode c, int pos, int np, Int32 aValue, Module aModule, Type[] aTypeGenericArgs, Type[] aMethodGenericArgs, ExceptionHandlingClause ehc)
            :base (c, pos, np, ehc)
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
