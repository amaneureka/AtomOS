using System;

namespace Atomix.Kernel_H.lib
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
