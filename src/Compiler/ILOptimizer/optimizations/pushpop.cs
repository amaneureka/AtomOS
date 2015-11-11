using System;
using System.Collections.Generic;

using Atomix.Assembler;
using Atomix.Assembler.x86;

namespace Atomix.ILOptimizer.optimizations
{
    public class pushpop : optimization
    {
        public override void Execute(List<Assembler.Instruction> Instructions)
        {
            for (int i = 1; i < Instructions.Count; i++)
            {   
                if (Instructions[i] is Pop)
                {
                    if (Instructions[i - 1] is Push)
                    {
                        var Next = (Pop)Instructions[i];
                        var Prev = (Push)Instructions[i - 1];
                        
                        var Optimized = new Mov();
                        if (Next.DestinationReg.HasValue)
                            Optimized.DestinationReg = Next.DestinationReg;
                        else
                        {
                            Console.WriteLine("Impossible Pair!");
                            Optimized.DestinationRef = Next.DestinationRef;
                        }

                        if (Prev.DestinationReg.HasValue)
                            Optimized.SourceReg = Prev.DestinationReg;
                        else
                            Optimized.SourceRef = Prev.DestinationRef;

                        //Inherit properties
                        Optimized.SourceIndirect = Prev.DestinationIndirect;
                        Optimized.SourceDisplacement = Prev.DestinationDisplacement;

                        if (Prev.Size != Next.Size && Next.Size != 32)
                            Console.WriteLine("Warning @push-pop");

                        Instructions[i - 1] = Optimized;
                        Instructions[i] = null;

                        //Case "Mov REG, REG" --> Remove this instruction then
                        if (Optimized.DestinationReg == Optimized.SourceReg
                            && Optimized.SourceIndirect == false
                            && Optimized.SourceDisplacement == 0)
                            Instructions[i - 1] = null;
                    }
                }
            }
        }
    }
}
