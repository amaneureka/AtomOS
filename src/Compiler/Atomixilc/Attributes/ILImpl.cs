using System;

namespace Atomixilc.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal class ILImplAttribute : Attribute
    {
        internal readonly ILCode OpCode;

        internal ILImplAttribute(ILCode aOpCode)
        {
            OpCode = aOpCode;
        }
    }
}
