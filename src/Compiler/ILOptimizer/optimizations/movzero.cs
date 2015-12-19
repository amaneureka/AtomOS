using System;
using System.Collections.Generic;

using Atomix.Assembler;
using Atomix.Assembler.x86;

namespace Atomix.ILOptimizer.optimizations
{
    public class movzero : optimization
    {
        public override void Execute(List<Assembler.Instruction> Instructions)
        {
            for (int index = 0; index < Instructions.Count; index++)
            {
                if (Instructions[index] is Mov)
                {
                    var xCurrent = Instructions[index] as Mov;
                    if (xCurrent.DestinationReg.HasValue 
                        && xCurrent.DestinationIndirect == false
                        && xCurrent.SourceRef == "0x0")
                    {
                        //Mov REG, 0 -> Xor REG, REG
                        Instructions[index] = new Xor { DestinationReg = xCurrent.DestinationReg, Size = xCurrent.Size, SourceReg = xCurrent.DestinationReg };
                    }
                }
            }
        }
    }
}
