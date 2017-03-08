/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          IQueue Generic Class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomix.Kernel_H.Core;

namespace Atomix.Kernel_H.Lib
{
    internal class IQueue<T>
    {
        T[] _items;
        int _size;
        int _capacity;

        internal IQueue(int capacity = 1)
        {
            _items = new T[capacity];
            _size = 0;
            _capacity = capacity;
        }

        internal void Enqueue(T item)
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

        internal T Dequeue()
        {
            var res = _items[0];
            for (int i = 1; i < _size; i++)
                _items[i - 1] = _items[i];
            _size--;
            return res;
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

        /// <summary>
        /// Free Internal Memory
        /// </summary>
        internal void Delete()
        {
            Heap.Free(_items);
        }
    }
}
