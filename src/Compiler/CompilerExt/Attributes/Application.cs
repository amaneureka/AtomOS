using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atomix.CompilerExt.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ApplicationAttribute : Attribute
    {
        protected CPUArch mCPUArch;

        #region Constructor
        public CPUArch CPUArch
        {
            get
            {
                return mCPUArch;
            }
        }
        #endregion

        public ApplicationAttribute(CPUArch CpuArch)
        {
            this.mCPUArch = CpuArch;
        }
    }
}
