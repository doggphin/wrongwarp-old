using System.Collections.Generic;
using UnityEngine;

namespace WrongWarp
{
    public class LIFOBuffer<T> : LinkedList<T>
    {
        private int capacity;

        public LIFOBuffer(int capacity)
        {
            this.capacity = capacity;
        }

        public void Add(T item)
        {
            if (Count == capacity) RemoveLast();
            AddFirst(item);
        }
    }
}
