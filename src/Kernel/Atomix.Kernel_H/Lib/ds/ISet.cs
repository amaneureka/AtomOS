/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          ISet Generic Class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomix.Kernel_H.Core;

namespace Atomix.Kernel_H.Lib
{
    internal class ISet<_type>
    {
        const uint Capacity = (1 << 5);// Should be a power of 2

        uint mModulo;
        Bucket[] mBuckets;

        HashFunction<_type> mFunction;
        EqualityFunction<_type> mEquality;

        class Bucket
        {
            public _type mKey;
            public Bucket mNext;
        }

        internal ISet(HashFunction<_type> aFunction, EqualityFunction<_type> aEquality)
        {
            mFunction = aFunction;
            mEquality = aEquality;
            mModulo = Capacity - 1;
            mBuckets = new Bucket[Capacity];
        }

        internal bool ContainsKey(_type mKey)
        {
            uint Index = mFunction(mKey) & mModulo;
            Bucket Current = mBuckets[Index];

            while (Current != null && !mEquality(Current.mKey, mKey))
                Current = Current.mNext;

            return (Current != null);
        }

        internal void RemoveKey(_type mKey)
        {
            uint Index = mFunction(mKey) & mModulo;
            Bucket Current = mBuckets[Index];

            if (Current == null)
                throw new Exception("[ISet]: Key not present!");

            if (mEquality(Current.mKey, mKey))
                mBuckets[Index] = Current.mNext;

            while (Current.mNext != null && !mEquality(Current.mNext.mKey, mKey))
                Current = Current.mNext;

            if (Current.mNext == null)
                throw new Exception("[ISet]: Key not present!");

            var ToDelete = Current.mNext;
            Current.mNext = ToDelete.mNext;
            Heap.Free(ToDelete);// Free bucket
        }
    }
}
