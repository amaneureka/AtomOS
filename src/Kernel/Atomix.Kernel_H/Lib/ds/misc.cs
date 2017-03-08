/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Datastructures Generic Class Helper
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomix.Kernel_H.Lib
{
    internal delegate uint HashFunction<_key>(_key athis);
    internal delegate bool EqualityFunction<_key>(_key a, _key b);
}
