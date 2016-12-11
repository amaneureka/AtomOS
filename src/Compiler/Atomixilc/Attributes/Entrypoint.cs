using System;
using System.Collections.Generic;

namespace Atomixilc.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class EntrypointAttribute : Attribute
    {
        public readonly string Entrypoint;
        public readonly Architecture Platform;

        public EntrypointAttribute(Architecture arch, string entrypoint)
        {
            Entrypoint = entrypoint;
            Platform = arch;
        }
    }
}
