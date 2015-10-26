using System;
using System.Collections.Generic;

using Atomix.Assembler;

namespace Atomix.ILOptimizer
{
    public abstract class optimization
    {
        public abstract void Execute(List<Instruction> Instructions);
    }
}
