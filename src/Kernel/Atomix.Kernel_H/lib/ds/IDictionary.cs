using System;

using Atomix.Kernel_H.lib.crypto;

namespace Atomix.Kernel_H.lib.ds
{
#warning Supported for strings only
    public class IDictionary<T_Value>
    {
        
        Bucket<T_Value>[] Buckets;
        uint[] Bucket_Size;
        const uint Capacity = (1 << 5);//Should be a power of 2
        
        public class Bucket<T_Value>
        {
            public string Key;
            public T_Value Value;
            public Bucket<T_Value> Next;
        }

        public IDictionary()
        {
            this.Buckets = new Bucket<T_Value>[Capacity];
            this.Bucket_Size = new uint[Capacity];
        }

        public T_Value this[string Key]
        {
            get
            {
                uint Hash = Key.GetsdbmHash();
                uint Index = Hash & (Capacity - 1);//Equivalent to (Hash % Capacity) if Capacity is power of 2
#warning What If hash is not in the buckets
                uint SIZE = Bucket_Size[Index];
                var Current = Buckets[Index];
                
                uint p = 0;
                while (p++ < SIZE && Current.Key != Key)
                    Current = Current.Next;
                return Current.Value;
            }
        }

        public void Add(string Key, T_Value Value)
        {
            uint Hash = Key.GetsdbmHash();
            uint Index = Hash & (Capacity - 1);

            uint SIZE = Bucket_Size[Index];

            var Current = Buckets[Index];
            if (SIZE == 0)
            {
                Current = new Bucket<T_Value>();
                Current.Key = Key;
                Current.Value = Value;
                Bucket_Size[Index] = 1;
                Buckets[Index] = Current;
                return;
            }

#warning What If same key is added twice            

            uint p = 1;
            while (p++ < SIZE)
                Current = Current.Next;

            var NewBucket = new Bucket<T_Value>();
            NewBucket.Key = Key;
            NewBucket.Value = Value;
            Bucket_Size[Index] = SIZE + 1;
            Current.Next = NewBucket;
        }

        public bool Contains(string Key)
        {
            uint Hash = Key.GetsdbmHash();
            uint Index = Hash & (Capacity - 1);

            uint SIZE = Bucket_Size[Index];
            var Current = Buckets[Index];

            uint p = 0;
            while (p++ < SIZE)
            {
                if (Current.Key == Key)
                    return true;
                Current = Current.Next;
            }

            return false;
        }
    }
}
