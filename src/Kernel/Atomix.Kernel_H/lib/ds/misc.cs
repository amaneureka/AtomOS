/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Datastructures Generic Class Helper
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomix.Kernel_H.Lib
{
    internal delegate uint HashFunction<_key>(_key athis);
    internal delegate bool EqualityFunction<_key>(_key a, _key b);
}
