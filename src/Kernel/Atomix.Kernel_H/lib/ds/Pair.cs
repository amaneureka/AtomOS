/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Pair Generic Class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomix.Kernel_H.Lib
{
    internal class Pair<A, B>
    {
        A aItem;
        B bItem;

        internal Pair(A item1, B item2)
        {
            aItem = item1;
            bItem = item2;
        }

        internal A First
        { get { return aItem; } }

        internal B Second
        { get { return bItem; } }
    }
}
