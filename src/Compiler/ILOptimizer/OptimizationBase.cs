/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
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
