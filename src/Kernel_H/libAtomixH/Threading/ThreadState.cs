using System;

namespace libAtomixH.Threading
{
    public enum ThreadState : int
    {
        None = -2,
        Dead = -1,
        Alive = 0
    };
}
