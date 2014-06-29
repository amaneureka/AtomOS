using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Atomix.CompilerExt.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class PlugAttribute : Attribute
    {
        protected CPUArch mTargetPlatform;
        protected string mTargetString;

        #region Constructor
        public CPUArch TargetPlatform
        {
            get
            {
                return mTargetPlatform;
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
        
        public PlugAttribute(string target, CPUArch tp = CPUArch.x86)
        {
            this.mTargetPlatform = tp;
            this.mTargetString = target;
        }
    }
}
