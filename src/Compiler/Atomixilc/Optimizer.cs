using System;
using System.Collections.Generic;
using System.Linq;

using Atomixilc.Machine;

namespace Atomixilc
{
    internal class Optimizer
    {
        internal Options Config;
        internal Stack<StackItem> vStack;

        Dictionary<Register, bool> NonVolatileRegisters;

        internal Optimizer(Options aConfig)
        {
            Config = aConfig;
            vStack = new Stack<StackItem>();
            NonVolatileRegisters = new Dictionary<Register, bool>();
        }

        void Setup()
        {
            NonVolatileRegisters.Add(Register.EBX, false);
            NonVolatileRegisters.Add(Register.ECX, false);
        }

        internal void FreeRegister(Register aReg)
        {
            if (NonVolatileRegisters.ContainsKey(aReg))
                NonVolatileRegisters[aReg] = false;
        }

        internal void AllocateRegister(Register aReg)
        {
            if (NonVolatileRegisters.ContainsKey(aReg))
            {
                if (NonVolatileRegisters[aReg])
                    throw new Exception("Tried to allocate already allocated register");
                NonVolatileRegisters[aReg] = true;
            }
            else
                Verbose.Error("Tried to Allocate VolatileRegister '{0}'", aReg);
        }

        internal bool GetNonVolatileRegister(ref Register? aFreeReg)
        {
            foreach(var regState in NonVolatileRegisters)
            {
                if (regState.Value == false)
                {
                    aFreeReg = regState.Key;
                    return true;
                }
            }
            return false;
        }

        internal void Reset()
        {
            vStack.Clear();
            NonVolatileRegisters.Clear();
            Setup();
        }
    }
}
