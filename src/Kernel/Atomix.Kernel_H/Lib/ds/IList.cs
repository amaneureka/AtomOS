/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          IList Generic Class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomix.Kernel_H.Core;

namespace Atomix.Kernel_H.Lib
{
    internal class IList<T>
    {
        T[] _items;
        int _size;
        int _capacity;

        internal IList(int capacity = 1)
        {
            _items = new T[capacity];
            _size = 0;
            _capacity = capacity;
        }

        internal void Add(T item)
        {
            if (_capacity <= _size)
            {
                var _new = new T[_size + _size];
                Array.Copy(_items, _new, _size);
                Heap.Free(_items);
                _items = _new;
                _capacity += _size;
            }
            _items[_size++] = item;
        }

        internal T this[int index]
        {
            get
            {
                return _items[index];
            }
            set
            {
                _items[index] = value;
            }
        }

        internal int Count
        {
            get
            {
                return _size;
            }
        }

        internal void Clear()
        {
            _size = 0;
        }

        internal void Delete()
        {
            Heap.Free(_items);
        }
    }
}
