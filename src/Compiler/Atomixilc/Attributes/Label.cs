using System;

namespace Atomixilc.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class LabelAttribute : Attribute
    {
        public readonly string RefLabel;

        public LabelAttribute(string label)
        {
            RefLabel = label;
        }
    }
}
