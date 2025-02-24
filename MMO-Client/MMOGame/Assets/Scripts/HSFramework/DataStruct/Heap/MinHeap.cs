using System;

namespace HSFramework.DataStruct
{
    public class MinHeap<T> : Heap<T> where T : IComparable
    {
        public MinHeap() : base(10, HeapType.MinHeap)
        {
        }
    }
}
