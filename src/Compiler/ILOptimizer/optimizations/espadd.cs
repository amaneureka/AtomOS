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
    /// ESP add optimization.
    /// Removes superfluous 'add esp, 0x**' instructions.
    /// </summary>
    public class espadd : OptimizationBase
    {

        /// <summary>
        /// Apply the optimizations.
        /// </summary>
        /// <param name="instructions">Instructions.</param>
        public override void Apply(List<Assembler.Instruction> instructions)
        {
            /*
             * Remove multiple entries of add esp, 0x**
             */
            for (int index = 0; index < instructions.Count; index++)
            {
                if (instructions[index] is Add)
                {
                    var xCurrentInstruction = (Add)instructions[index];
                    int StackFlushCount = 0;

                    try { StackFlushCount = Convert.ToInt32(xCurrentInstruction.SourceRef, 16); }
                    //Not a number, So move on
                    catch {  continue; }

                    if (ValidEntry(xCurrentInstruction))
                    {
                        for (int index2 = index + 1; index2 < instructions.Count; index2++)
                        {
                            if (ValidEntry(instructions[index2] as Add))
                            {
                                StackFlushCount += Convert.ToInt32(((Add)instructions[index2]).SourceRef, 16);
                                instructions[index2] = null;
                            }
                            else
                            {
                                break;
                            }
                            index = index2;
                        }
                    }
                    xCurrentInstruction.SourceRef = "0x" + StackFlushCount.ToString("X");
                }
            }
        }

        private static bool ValidEntry(Add xCurrentInstruction)
        {
            if (xCurrentInstruction == null)
                return false;

            if (xCurrentInstruction.DestinationReg == Registers.ESP
                        && xCurrentInstruction.DestinationIndirect == false)
            {
                return true;
            }
            return false;
        }
    }
}
