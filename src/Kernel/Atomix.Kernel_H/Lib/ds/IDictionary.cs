/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          IDictionay Generic Class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomix.Kernel_H.Core;

namespace Atomix.Kernel_H.Lib
{
    internal class IDictionary<_key, _value>
    {
        const uint Capacity = (1 << 5);// Should be a power of 2

        uint mModulo;
        Bucket[] mBuckets;

        HashFunction<_key> mFunction;
        EqualityFunction<_key> mEquality;

        class Bucket
        {
            public _key mKey;
            public _value mValue;
            public Bucket mNext;
        }

        internal IDictionary(HashFunction<_key> aFunction, EqualityFunction<_key> aEquality)
        {
            mFunction = aFunction;
            mEquality = aEquality;
            mModulo = Capacity - 1;
            mBuckets = new Bucket[Capacity];
        }

        internal _value this[_key aKey]
        {
            get
            {
                uint Index = mFunction(aKey) & mModulo;
                Bucket Current = mBuckets[Index];

                while (Current != null && !mEquality(Current.mKey, aKey))
                    Current = Current.mNext;

                if (Current == null || !mEquality(Current.mKey, aKey))
                    throw new Exception("[IDictionary]: Key not found!");

                return Current.mValue;
            }
        }

        internal _value GetValue(_key aKey, _value defaultValue)
        {
            uint Index = mFunction(aKey) & mModulo;
            Bucket Current = mBuckets[Index];

            while (Current != null && !mEquality(Current.mKey, aKey))
                Current = Current.mNext;

            if (Current == null || !mEquality(Current.mKey, aKey))
                return defaultValue;

            return Current.mValue;
        }

        internal void Add(_key aKey, _value aValue)
        {
            if (SafeAdd(aKey, aValue))
                return;
            throw new Exception("[IDictionary]: Key Already present!");
        }

        internal bool SafeAdd(_key aKey, _value aValue)
        {
            uint Index = mFunction(aKey) & mModulo;
            Bucket Current = mBuckets[Index];

            Bucket NewBucket = new Bucket
            {
                mKey = aKey,
                mValue = aValue,
                mNext = null
            };

            if (Current == null)
            {
                mBuckets[Index] = NewBucket;
                return true;
            }

            while (Current.mNext != null && !mEquality(Current.mKey, aKey))
                Current = Current.mNext;

            if (Current.mNext != null)
                return false;

            Current.mNext = NewBucket;
            return true;
        }

        internal bool ContainsKey(_key aKey)
        {
            uint Index = mFunction(aKey) & mModulo;
            Bucket Current = mBuckets[Index];

            while (Current != null && !mEquality(Current.mKey, aKey))
                Current = Current.mNext;

            if (Current == null)
                return false;

            return true;
        }

        internal void RemoveKey(_key mKey)
        {
            uint Index = mFunction(mKey) & mModulo;
            Bucket Current = mBuckets[Index];

            if (Current == null)
                throw new Exception("[IDictionary]: Key not present!");

            Bucket ToDelete;
            if (mEquality(Current.mKey, mKey))
            {
                mBuckets[Index] = Current.mNext;
                ToDelete = Current;
            }
            else
            {
                while (Current.mNext != null && !mEquality(Current.mNext.mKey, mKey))
                    Current = Current.mNext;

                if (Current.mNext == null)
                    throw new Exception("[IDictionary]: Key not present!");

                ToDelete = Current.mNext;
                Current.mNext = ToDelete.mNext;
            }
            Heap.Free(ToDelete);// Free bucket
        }
    }
}
