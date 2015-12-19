using System;
using System.Collections.Generic;

using Atomix.Assembler;
using Atomix.Assembler.x86;

using Atomix.ILOptimizer.optimizations;

namespace Atomix.ILOptimizer
{
    public class Worker
    {
        private List<Instruction> Assembly;
        private List<Instruction> OptimizedAssembly;
        public const string OPTIMIZATION_START_FLAG = "__do_optimization__";
        public const string OPTIMIAZTION_END_FLAG = "__do_end_optimization__";
        public List<optimization> Algorithms;

        public Worker(List<Instruction> mAssembly)
        {
            this.Assembly = mAssembly;
            this.OptimizedAssembly = new List<Instruction>();
            this.Algorithms = new List<optimization>();
            LoadAlgorithms();
        }

        
        private void LoadAlgorithms()
        {
            Algorithms.Add(new pushpop());
            Algorithms.Add(new espadd());
            Algorithms.Add(new ebppush());
            Algorithms.Add(new movzero());
            //Algorithms.Add(new movmov());
        }

        public List<Instruction> Start()
        {
            bool start_position = false;
            var Temporary = new List<Instruction>();

            for (int opcode = 0; opcode < Assembly.Count; opcode++)
            {
                var instruction = Assembly[opcode];
                if (instruction is Comment)
                {
                    var comment = ((Comment)instruction).Comments;
                    if (comment == OPTIMIZATION_START_FLAG)
                    {
                        Temporary.Clear();
                        start_position = true;
                        continue;
                    }
                    else if (comment == OPTIMIAZTION_END_FLAG)
                    {
                        foreach (var algo in Algorithms)
                        {
                            algo.Execute(Temporary);
                            //Clean up!
                            for (int index = 0; index < Temporary.Count; )
                            {
                                if (Temporary[index] == null)
                                {
                                    Temporary.RemoveAt(index);
                                    continue;
                                }
                                index++;
                            }
                        }
                        OptimizedAssembly.Add(new Comment(OPTIMIZATION_START_FLAG));
                        OptimizedAssembly.AddRange(Temporary);
                        OptimizedAssembly.Add(new Comment(OPTIMIAZTION_END_FLAG));
                        start_position = false;
                        continue;
                    }
                }

                if (start_position == false)
                    OptimizedAssembly.Add(instruction);
                else
                    Temporary.Add(instruction);
            }
            return OptimizedAssembly;
        }
    }
}
