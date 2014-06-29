using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Atomix
{
    public abstract class ILOpCode
    {
        public ILCode Code;

        public int Position;
        public int NextPosition;
        public ExceptionHandlingClause Ehc;

        public ILOpCode(ILCode c, int pos, int np, ExceptionHandlingClause ehc)
        {
            this.Code = c;
            this.Position = pos;
            this.NextPosition = np;
            this.Ehc = ehc;
        }
                
    }    
}
