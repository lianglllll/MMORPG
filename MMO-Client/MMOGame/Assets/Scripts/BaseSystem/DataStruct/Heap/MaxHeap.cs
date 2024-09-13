using System;

namespace BaseSystem.DataStruct
{
    public class MaxHeap<T> : Heap<T> where T : IComparable
    {
        public MaxHeap() : base(10, HeapType.MaxHeap)
        {
        }
    }
}
