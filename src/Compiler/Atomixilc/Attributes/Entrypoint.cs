/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Application Entrypoint Attribute
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

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
