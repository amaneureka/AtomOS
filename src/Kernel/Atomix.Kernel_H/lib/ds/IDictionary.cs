/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* Copyright (c) 2015, Atomix Development, Inc - All Rights Reserved                                        *
*                                                                                                          *
* Unauthorized copying of this file, via any medium is strictly prohibited                                 *
* Proprietary and confidential                                                                             *
* Written by Aman Priyadarshi <aman.eureka@gmail.com>, December 2015                                       *
*                                                                                                          *
*   Namespace     ::  Atomix.Kernel_H.lib                                                                  *
*   File          ::  IDictionary.cs                                                                       *
*                                                                                                          *
*   Description                                                                                            *
*       File Contains Dictionary implementation                                                            *
*                                                                                                          *
*   History                                                                                                *
*       20-12-2015      Aman Priyadarshi      Basic Implementation                                         *
*       24-03-2016      Aman Priyadarshi      Fully qualified Dictionary Implementaion and File Header     *
*                                                                                                          *
* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;

namespace Atomix.Kernel_H.lib
{
    public delegate uint HashFunction<_key>(_key athis);
    public delegate bool EqualityFunction<_key>(_key a, _key b);

    public class IDictionary<_key, _value>
    {
        const uint Capacity = (1 << 5);//Should be a power of 2

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

        public IDictionary(HashFunction<_key> aFunction, EqualityFunction<_key> aEquality)
        {
            mFunction = aFunction;
            mEquality = aEquality;
            mModulo = Capacity - 1;
            mBuckets = new Bucket[Capacity];
        }

        public _value this[_key aKey]
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

        public void Add(_key aKey, _value aValue)
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
                return;
            }
            
            while (Current.mNext != null && !mEquality(Current.mKey, aKey))
                Current = Current.mNext;

            if (Current.mNext != null)
                throw new Exception("[IDictionary]: Key Already Present!");

            Current.mNext = NewBucket;
        }
        
        public bool Contains(_key aKey)
        {
            uint Index = mFunction(aKey) & mModulo;
            Bucket Current = mBuckets[Index];
                        
            while (Current != null && !mEquality(Current.mKey, aKey))
                Current = Current.mNext;
            
            if (Current == null)
                return false;

            return true;
        }
    }
}
