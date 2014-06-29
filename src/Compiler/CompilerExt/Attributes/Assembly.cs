using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atomix.CompilerExt.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class AssemblyAttribute : Attribute
    {        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">It is (Sum of size of all argument - Size of return)</param>
        public AssemblyAttribute(uint x = 0xFF) 
        { }
    }
}
