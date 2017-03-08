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
    /// MOV zero optimization.
    /// Replaces 'MOV [register], 0' with the faster 'XOR [register], [register]' instruction.
    /// </summary>
    public class movzero : OptimizationBase
    {

        /// <summary>
        /// Apply the optimization.
        /// </summary>
        /// <param name="instructions">Instructions.</param>
        public override void Apply(List<Assembler.Instruction> instructions)
        {

            // Iterate over the instructions
            for (int index = 0; index < instructions.Count; index++)
            {
                var instr = instructions [index] as Mov;

                // Continue if the instruction is not a 'MOV' instruction.
                if (instr == null)
                    continue;

                // Test if all requirements are met
                if (instr.DestinationReg.HasValue
                    && !instr.DestinationIndirect
                    && instr.SourceRef == "0x0") {

                    // Replace 'MOV' by faster 'XOR'
                    instructions [index] = new Xor {
                        SourceReg = instr.DestinationReg,
                        DestinationReg = instr.DestinationReg,
                        Size = instr.Size
                    };
                }
            }
        }
    }
}
