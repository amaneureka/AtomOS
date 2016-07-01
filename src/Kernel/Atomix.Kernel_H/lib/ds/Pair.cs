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
    public class Pair<A, B>
    {
        A aItem;
        B bItem;

        public Pair(A item1, B item2)
        {
            this.aItem = item1;
            this.bItem = item2;
        }

        public A First
        { get { return aItem; } }

        public B Second
        { get { return bItem; } }
    }
}
