using System;

namespace Atomixilc.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class AssemblyAttribute : Attribute
    {
        public readonly bool CalliHeader;

        public AssemblyAttribute(bool requiredCallingHeader = false)
        {
            CalliHeader = requiredCallingHeader;
        }
    }
}
