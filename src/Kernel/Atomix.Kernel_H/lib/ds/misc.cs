using System;

namespace Atomix.Kernel_H.lib
{
    public delegate uint HashFunction<_key>(_key athis);
    public delegate bool EqualityFunction<_key>(_key a, _key b);
}
