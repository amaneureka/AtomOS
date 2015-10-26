using System;
using System.Collections.Generic;
using Atomix.Assembler;
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
        }

        public List<Instruction> Start()
        {
            int start_position = -1;
            List<Instruction> Temporary = new List<Instruction>();
            for (int i = 0; i < Assembly.Count; i++)
            {
                var instruction = Assembly[i];
                if (instruction is Comment)
                {
                    var comment = ((Comment)instruction).Comments;
                    if (comment == OPTIMIZATION_START_FLAG)
                    {
                        Temporary.Clear();
                        start_position = i;
                        continue;
                    }
                    else if (comment == OPTIMIAZTION_END_FLAG)
                    {
                        foreach(var algo in Algorithms)
                            algo.Execute(Temporary);
                        OptimizedAssembly.Add(new Comment(OPTIMIZATION_START_FLAG));
                        OptimizedAssembly.AddRange(Temporary);
                        OptimizedAssembly.Add(new Comment(OPTIMIAZTION_END_FLAG));
                        start_position = -1;
                        continue;
                    }
                }

                if (start_position == -1)
                    OptimizedAssembly.Add(instruction);
                else
                    Temporary.Add(instruction);
            }
            return OptimizedAssembly;
        }
    }
}
