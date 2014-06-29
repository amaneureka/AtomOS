using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Atomix.CompilerExt;
using Atomix.Assembler;

namespace Atomix.Assembler.x86
{
    public class In : DestinationSource
    {
        public In()
            : base("in") { }        
    }
}
