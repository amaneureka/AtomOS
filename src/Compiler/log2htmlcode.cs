/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Compiler Logger template class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System.Collections;

namespace Atomix
{
    public partial class log2html
    {
        ArrayList mScript;
        ArrayList mMessage;
        ArrayList mDetail;

        string mExecuteTime;

        public log2html(ArrayList aScript, ArrayList aMessage, ArrayList aDetail, string aExecutionTime)
        {
            mScript = aScript;
            mMessage = aMessage;
            mDetail = aDetail;
            mExecuteTime = aExecutionTime;
        }
    }
}
