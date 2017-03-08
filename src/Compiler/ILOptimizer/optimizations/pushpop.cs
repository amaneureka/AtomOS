/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Optimization
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;
using System.Collections.Generic;

using Atomix.Assembler;
using Atomix.Assembler.x86;

namespace Atomix.ILOptimizer.Optimizations
{
    /// <summary>
    /// PUSP / POP optimization.
    /// </summary>
    public class pushpop : OptimizationBase
    {

        /// <summary>
        /// Apply the optimization.
        /// </summary>
        /// <param name="instructions">Instructions.</param>
        public override void Apply(List<Assembler.Instruction> instructions)
        {

            // Iterate over the instructions
            for (int i = 1; i < instructions.Count; i++)
            {

                // Test if the current instruction is a 'POP' instruction
                if (instructions[i] is Pop)
                {

                    // Test if the previous instruction is a 'PUSH' instruction
                    if (instructions[i - 1] is Push)
                    {
                        var Next = (Pop)instructions[i];
                        var Prev = (Push)instructions[i - 1];

                        var Optimized = new Mov();
                        if (Next.DestinationReg.HasValue)
                            Optimized.DestinationReg = Next.DestinationReg;
                        else
                        {
                            Console.WriteLine("@push-pop: Impossible Pair!");
                            Optimized.DestinationRef = Next.DestinationRef;
                        }

                        if (Prev.DestinationReg.HasValue)
                            Optimized.SourceReg = Prev.DestinationReg;
                        else
                            Optimized.SourceRef = Prev.DestinationRef;

                        // Inherit properties
                        Optimized.SourceIndirect = Prev.DestinationIndirect;
                        Optimized.SourceDisplacement = Prev.DestinationDisplacement;

                        if (Prev.Size != Next.Size && Next.Size != 32)
                            Console.WriteLine("@push-pop: Warning");

                        instructions[i - 1] = Optimized;
                        instructions[i] = null;

                        // Case "Mov REG, REG" --> Remove this instruction then
                        if (Optimized.DestinationReg == Optimized.SourceReg
                            && Optimized.SourceIndirect == Optimized.DestinationIndirect
                            && Optimized.SourceDisplacement == Optimized.DestinationDisplacement)
                            instructions[i - 1] = null;
                    }
                }
            }
        }
    }
}
