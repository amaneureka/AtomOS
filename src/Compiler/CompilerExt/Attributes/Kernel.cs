using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atomix.CompilerExt.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class KernelAttribute : Attribute
    {
        protected CPUArch mCPUArch;
        protected string mOrganize;
        #region Constructor
        public CPUArch CPUArch
        {
            get
            {
                return mCPUArch;
            }
        }
        #endregion

        public KernelAttribute(CPUArch CpuArch, string Organize)
        {
            this.mCPUArch = CpuArch;
            this.mOrganize = Organize;
        }        
    }
}
