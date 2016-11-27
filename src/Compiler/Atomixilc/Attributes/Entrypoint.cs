using System;
using System.Collections.Generic;

namespace Atomixilc.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class EntrypointAttribute : Attribute
    {
        public readonly Architecture Platform;

        public EntrypointAttribute(Architecture arch)
        {
            Platform = arch;
        }
    }
}
