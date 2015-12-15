using System;
using System.Collections.Generic;

using Atomix.Assembler;
using Atomix.Assembler.x86;

namespace Atomix.ILOptimizer.optimizations
{
    public class ebppush : optimization
    {
        public override void Execute(List<Assembler.Instruction> Instructions)
        {
            //If a EBP relative push is done, then we can safely replace it by mov instruction
            for (int index = 0; index < Instructions.Count; index++)
            {
                if (Instructions[index] is Push)
                {
                    var Prev = Instructions[index] as Push;
                    if (Prev.DestinationReg != Registers.EBP && Prev.DestinationReg.HasValue)
                        continue;

                    //Search for the pop instruction
                    int index2 = index + 1;
                    int StackLevel = 0;
                    for (; index2 < Instructions.Count; index2++)
                    {
                        var xNext = Instructions[index2];
                        #region We Can't Optimize these cases
                        if (xNext is Label || xNext is Jmp || xNext is Call)
                        {
                            index2 = Instructions.Count;
                            break;
                        }
                        if (xNext is OnlyDestination)
                        {
                            var xtemp = xNext as OnlyDestination;
                            if (xtemp.DestinationReg == Registers.ESP)
                            {
                                index2 = Instructions.Count;
                                break;
                            }
                        }
                        if (xNext is DestinationSource)
                        {
                            var xtemp = xNext as DestinationSource;
                            if (xtemp.DestinationReg == Registers.ESP || xtemp.SourceReg == Registers.ESP)
                            {
                                index2 = Instructions.Count;
                                break;
                            }
                        }
                        if (xNext is DestinationSourceSize)
                        {
                            var xtemp = xNext as DestinationSourceSize;
                            if (xtemp.DestinationReg == Registers.ESP || xtemp.SourceReg == Registers.ESP)
                            {
                                index2 = Instructions.Count;
                                break;
                            }
                        }
                        #endregion
                        if (Instructions[index2] is Push)
                            StackLevel++;
                        if (Instructions[index2] is Pop)
                        {
                            if (StackLevel == 0)
                                break;
                            --StackLevel;
                        }
                    }

                    if (index2 == Instructions.Count)
                        continue;//Nope, No way to optimize this

                    var Next = Instructions[index2] as Pop;

                    var Optimized = new Mov();
                    if (Next.DestinationReg.HasValue)
                        Optimized.DestinationReg = Next.DestinationReg;
                    else
                    {
                        Console.WriteLine("@EBP-Push: Impossible Pair!");
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
                        Console.WriteLine("@push-pop: Warning");

                    Instructions[index2] = Optimized;
                    Instructions[index] = null;
                }
            }
        }
    }
}
