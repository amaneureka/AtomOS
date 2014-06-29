using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atomix
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ILOpAttribute : Attribute
    {
        //Just its presense is necessary
        public ILOpAttribute(ILCode Il) { }
    }
}
