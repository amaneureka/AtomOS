/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Optimization worker
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;
using System.Collections.Generic;

using Atomix.Assembler;

using Atomix.ILOptimizer.Optimizations;

namespace Atomix.ILOptimizer
{

    /// <summary>
    /// Worker.
    /// </summary>
    public class Worker
    {

        /// <summary>
        /// The input instructions.
        /// </summary>
        private readonly List<Instruction> Assembly;

        /// <summary>
        /// The (optimized) output instructions.
        /// </summary>
        private readonly List<Instruction> OptimizedAssembly;

        /// <summary>
        /// The optimization start flag.
        /// </summary>
        public const string OPTIMIZATION_START_FLAG = "__do_optimization__";

        /// <summary>
        /// The optimiaztion end flag.
        /// </summary>
        public const string OPTIMIAZTION_END_FLAG = "__do_end_optimization__";

        /// <summary>
        /// The optimization algorithms.
        /// </summary>
        public List<OptimizationBase> Algorithms;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Atomix.ILOptimizer.Worker"/> class.
        /// </summary>
        /// <param name="aAssembly">The instructions that make the assembly.</param>
        public Worker(List<Instruction> aAssembly)
        {
            Assembly = aAssembly;
            OptimizedAssembly = new List<Instruction>();
            Algorithms = new List<OptimizationBase>();

            // Load the algorithms
            LoadAlgorithms();
        }

        /// <summary>
        /// Loads the algorithms.
        /// </summary>
        /// <returns>The algorithms.</returns>
        private void LoadAlgorithms()
        {
            Algorithms.Add(new pushpop());
            Algorithms.Add(new espadd());
            Algorithms.Add(new ebppush());
            Algorithms.Add(new movzero());
            //Algorithms.Add(new movmov());
        }

        /// <summary>
        /// Optimizes code based on the loaded algorithms.
        /// </summary>
        public List<Instruction> Start()
        {
            bool start_position = false;
            var Temporary = new List<Instruction>();

            // Iterate over the opcodes
            for (int opcode = 0; opcode < Assembly.Count; opcode++)
            {

                // Fetch the instruction
                var instruction = Assembly[opcode];

                // Test if the instruction is a comment
                if (instruction is Comment)
                {

                    // Get the comment
                    var comment = ((Comment)instruction).Comments;

                    // Test if the comment is the optimization start flag
                    if (comment == OPTIMIZATION_START_FLAG)
                    {

                        // Clear temporaries
                        Temporary.Clear();
                        start_position = true;
                        continue;
                    }

                    // Test if the comment is the optimization end flag
                    if (comment == OPTIMIAZTION_END_FLAG)
                    {

                        // Iterate over the algorithms
                        foreach (var algo in Algorithms)
                        {

                            // Apply all optimizations on the temporaries
                            algo.Apply(Temporary);

                            // Clean up!
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

                        // Add the optimizations to the optimized assembly
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

            // Return the optimized assembly
            return OptimizedAssembly;
        }
    }
}
