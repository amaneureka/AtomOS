using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace Atomix
{
    public partial class log2html
    {
        ArrayList Ascript;
        ArrayList Amessage;
        ArrayList Adetail;

        string ExecuteTime = string.Empty;

        public log2html(ArrayList ascript, ArrayList amessage, ArrayList adetail, string executetime)
        {
            Ascript = ascript;
            Amessage = amessage;
            Adetail = adetail;
            ExecuteTime = executetime;
        }
    }
}
