/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Optimization
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;
using System.Collections.Generic;

using Atomix.Assembler;

namespace Atomix.ILOptimizer
{

    /// <summary>
    /// Optimization base class.
    /// </summary>
    public abstract class OptimizationBase
    {

        /// <summary>
        /// Apply the optimization.
        /// </summary>
        /// <param name="instructions">Instructions.</param>
        public abstract void Apply(List<Instruction> instructions);
    }
}
