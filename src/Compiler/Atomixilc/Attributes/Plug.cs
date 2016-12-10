using System;

namespace Atomixilc.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class PlugAttribute : Attribute
    {
        public readonly string TargetLabel;
        public readonly Architecture Platform;

        public PlugAttribute(string label, Architecture arch = Architecture.x86)
        {
            TargetLabel = label;
            Platform = arch;
        }
    }
}
