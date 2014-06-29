using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atomix.CompilerExt.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PlugClassAttribute : Attribute
    {
        protected CPUArch mTargetPlatform;
        protected Type mTargetType;
        protected string mTargetString;

        #region Constructor
        public CPUArch TargetPlatform
        {
            get
            {
                return mTargetPlatform;
            }
        }

        public Type TargetType
        {
            get
            {
                return mTargetType;
            }
        }

        public string TargetString
        {
            get
            {
                return mTargetString;
            }
        }
        #endregion

        public PlugClassAttribute(CPUArch tp, Type target)
        {            
            this.mTargetPlatform = tp;
            this.mTargetType = target;
        }

        public PlugClassAttribute(CPUArch tp, string target)
        {
            this.mTargetPlatform = tp;
            this.mTargetString = target;
        }
    }
}
