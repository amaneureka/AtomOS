using System;
using System.Collections.Generic;

using Atomixilc.Machine;

namespace Atomixilc
{
    internal class StackItem
    {
        internal Register? RegisterRef;
        internal string AddressRef;
        internal bool IsIndirect;
        internal int Displacement;
    }
}
