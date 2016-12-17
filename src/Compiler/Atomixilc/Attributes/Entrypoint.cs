/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
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
