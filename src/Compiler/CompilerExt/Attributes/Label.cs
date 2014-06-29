using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Atomix.CompilerExt.Attributes
{
    [AttributeUsage(AttributeTargets.All , AllowMultiple = false)]
    public class LabelAttribute : Attribute
    {
        protected string mLabel;

        #region Constructor
        public string Label
        {
            get
            {
                return mLabel;
            }
        }
        #endregion

        public LabelAttribute(string xLabel)
        {
            this.mLabel = xLabel;
        }
    }
}
