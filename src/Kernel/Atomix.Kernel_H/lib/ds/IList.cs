/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          IList Generic Class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomix.Kernel_H.core;

namespace Atomix.Kernel_H.lib
{
    public class IList<T>
    {
        T[] _items;
        int _size;
        int _capacity;

        public IList(int capacity = 1)
        {
            _items = new T[capacity];
            _size = 0;
            _capacity = capacity;
        }

        public void Add(T item)
        {
            if (_capacity <= _size)
            {
                var _new = new T[_size + 1];
                Array.Copy(_items, _new, _size);
                Heap.Free(_items);
                _items = _new;
                _capacity++;
            }
            _items[_size++] = item;
        }

        public void Refresh()
        {
            int _l = 0;
            for (int i = 0; i < _size; i++)
            {
                if (_items[i] != null)
                {
                    _items[_l++] = _items[i];
                }
            }
            _size = _l;
        }

        public T this[int index]
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

        public int Count
        {
            get
            {
                return _size;
            }
        }

        public void Clear()
        {
            _size = 0;
        }

        /// <summary>
        /// Free Internal Memory
        /// </summary>
        public void Delete()
        {
            Heap.Free(_items);
        }
    }
}
