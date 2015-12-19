using System;
using System.Collections.Generic;

using Atomix.Assembler;
using Atomix.Assembler.x86;

namespace Atomix.ILOptimizer.optimizations
{
    public class espadd : optimization
    {
        public override void Execute(List<Assembler.Instruction> Instructions)
        {
            /*
             * Remove multiple entries of add esp, 0x**
             */
            for (int index = 0; index < Instructions.Count; index++)
            {
                if (Instructions[index] is Add)
                {
                    var xCurrentInstruction = (Add)Instructions[index];
                    int StackFlushCount = 0;

                    try { StackFlushCount = Convert.ToInt32(xCurrentInstruction.SourceRef, 16); }
                    //Not a number, So move on
                    catch {  continue; }
                    
                    if (ValidEntry(xCurrentInstruction))
                    {
                        for (int index2 = index + 1; index2 < Instructions.Count; index2++)
                        {
                            if (ValidEntry(Instructions[index2] as Add))
                            {
                                StackFlushCount += Convert.ToInt32(((Add)Instructions[index2]).SourceRef, 16);
                                Instructions[index2] = null;
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
