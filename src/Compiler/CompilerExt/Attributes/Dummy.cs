using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atomix.CompilerExt.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class DummyAttribute : Attribute
    {
        /* How it works? So it just make the method invisible for compiler and reflect nothing if ID = 0
         * else it will reflect the method which has plug code label of same id
         */
        /// <summary>
        /// The ID
        /// </summary>
        protected uint mID;

        #region Constructor
        public uint ID
        {
            get
            {
                return mID;
            }
        }
        #endregion

        public DummyAttribute(uint xID = 0) 
        {
            this.mID = xID;
        }
    }
}
