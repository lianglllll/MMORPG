using System;
using System.Collections.Generic;
using System.Text;

namespace BaseSystem.DataStruct
{
    public enum HeapType
    {
        MinHeap,
        MaxHeap
    }

    public class Heap<T> : IDisposable where T : IComparable
    {
        private bool _disposed = false;
        protected List<T> itemArray;
        private int capacity;
        protected int count;
        public int Count => count;
        private readonly Func<T, T, bool> _comparerFun;

        public Heap(int capacity, HeapType heapType)
        {
            if (heapType == HeapType.MinHeap)
                _comparerFun = MinComparerFunc;
            else
                _comparerFun = MaxComparerFunc;

            Init(capacity);
        }

        public bool CheckHeapValid()
        {
            if (IsEmpty() || Count == 1) // 如果堆为空或仅有一个元素，则认为堆满足最大堆或最小堆的条件
                return true;

            for (int i = 2; i <= Count; i++)
            {
                if (_comparerFun(itemArray[i / 2], itemArray[i]))
                    return false;
            }

            return true;
        }

        private bool MinComparerFunc(T t1, T t2)
        {
            return t1.CompareTo(t2) > 0;
        }

        private bool MaxComparerFunc(T t1, T t2)
        {
            return t1.CompareTo(t2) < 0;
        }

        void Init(int initCapacity)
        {
            if (initCapacity <= 0)
            {
                throw new IndexOutOfRangeException();
            }

            capacity = initCapacity;
            //从下标为1开始存放数据
            itemArray = new List<T>(initCapacity + 1) { default };
            count = 0;
        }

        public bool HasItem(T item)
        {
            if (IsEmpty())
                return false;
            for (int i = 1; i <= count; i++)
            {
                if (itemArray[i].CompareTo(item) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        public int GetItemIndex(T item)
        {
            int index = -1;
            for (int i = 1; i <= count; i++)
            {
                if (itemArray[i].CompareTo(item) == 0)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        /// <summary>
        /// 堆是否已经满
        /// </summary>
        /// <returns></returns>
        public bool IsFull()
        {
            return count == capacity;
        }

        public bool IsEmpty()
        {
            return count == 0;
        }

        public void Insert(T item)
        {
            //i指向插入堆后的最后一个元素位置
            itemArray.Add(item);
            count += 1;
            Pop(count);
        }

        private void Pop(int index)
        {
            T targetItem = itemArray[index];
            while (index > 1 && _comparerFun(itemArray[index / 2], targetItem))
            {
                var parentIndex = index / 2;
                itemArray[index] = itemArray[parentIndex];
                index = parentIndex;
            }

            itemArray[index] = targetItem;
        }

        protected void Sink(int index)
        {
            T targetItem = itemArray[index];
            int parent = index;
            //节点i的左儿子下标为2*i，右儿子下标为2*i+1
            while (parent * 2 <= count)
            {
                var child = parent * 2;
                //Min:让Child指向左右节点中较小的那个
                //Max:让Child指向左右节点中较大的那个
                if (child != count && _comparerFun(itemArray[child], itemArray[child + 1]))
                    child++;
                if (_comparerFun(itemArray[child], targetItem))
                    break;
                itemArray[parent] = itemArray[child];
                //将temp元素移动到下一层
                //child移动到parent位置了，所以接下来需要从其他地方移动数据到child位置上。这里直接循环即可
                parent = child;
            }

            itemArray[parent] = targetItem;
        }

        public void Adjust(T item)
        {
            //如果Item有父节点
            int index = GetItemIndex(item);
            //如果不包含这个节点
            if (index == -1)
                return;
            //Min: 如果item变化后比父节点大  old: root < item < child   now: root < item  但是child和item关系不确定
            //Max: //如果item变化后比父节点小  old: root > item > child   now: root > item  但是child和item关系不确定
            if (_comparerFun(item, itemArray[index / 2]))
            {
                //调整item与子节点的
                Sink(index);
            }
            else
            {
                //oldMin: root < item < child  now:item < root < child  root已经没有资格再做root了，需要往上冒
                //oldMax: root > item > child  now:item > root > child  root已经没有资格再做root了，需要往上冒
                Pop(index);
            }
        }

        public bool Delete(T item)
        {
            if (IsEmpty())
                throw new InvalidOperationException("Heap is empty");

            int index = GetItemIndex(item);
            if (index == -1)
                return false;

            // 如果只有一个元素，直接删除
            if (count == 1)
            {
                itemArray.RemoveAt(1);
                count--;
                return true;
            }
            
            //删除的就是最后一个元素
            if (index == count)
            {
                itemArray.RemoveAt(count);
                count--;
                return true;
            }
            // 将要删除的元素和堆底元素交换
            T lastItem = itemArray[count];
            itemArray[count] = itemArray[index];
            itemArray[index] = lastItem;
            itemArray.RemoveAt(count);
            count--;

            // 根据堆类型进行不同的操作
            if (_comparerFun(lastItem, item))
            {
                Sink(index); // 最小堆下沉
            }
            else
            {
                Pop(index); // 最大堆上浮
            }

            return true;
        }

        public T DeleteHead()
        {
            if (IsEmpty())
                throw new IndexOutOfRangeException();
            T deleteItem = itemArray[1];
            if (count > 1)
                itemArray[1] = itemArray[count];
            itemArray.RemoveAt(count);
            count -= 1;
            if (count > 1)
                Sink(1);
            return deleteItem;
        }

        public T GetHead()
        {
            if (IsEmpty())
            {
                throw new IndexOutOfRangeException("Heap is empty!");
            }

            return itemArray[1];
        }

        public string GetString(string splitChar = "  ")
        {
            if (itemArray == null || itemArray.Count <= 1)
            {
                return string.Empty;
            }

            StringBuilder result = new StringBuilder();

            int h = (int)Math.Floor(Math.Log(itemArray.Count, 2)) + 1; // 最小堆的高度
            int index = 1; // 当前节点在数组中的下标
            int maxIndexInCurLevel = 1; // 当前层级最大节点在数组中的下标

            for (int i = 0; i < h; i++)
            {
                // 遍历每一层
                // 输出左边的空格
                for (int j = 0; j < (1 << (h - i)) - 2; j++)
                    result.Append(splitChar);

                for (int j = index; j <= Math.Min(itemArray.Count - 1, maxIndexInCurLevel); j++)
                {
                    // 遍历当前层级的所有节点
                    result.Append(itemArray[j]);
                    //右边的空格
                    for (int k = 0; k < (1 << (h - i + 1)) - 2; k++)
                        result.Append(splitChar);
                }

                result.Append("\n"); // 换行
                index = maxIndexInCurLevel + 1;
                maxIndexInCurLevel = (maxIndexInCurLevel + 1) * 2 - 1;
            }

            return result.ToString();
        }


        public override string ToString()
        {
            return GetString();
        }

        public List<T>.Enumerator GetHeapEnumerator()
        {
            return itemArray.GetEnumerator();
        }

        public void Clear()
        {
            itemArray.Clear();
            itemArray.Add(default);
            count = 0;
        }

        public void Dispose()
        {
            // 调用Dispose(true)释放托管和非托管资源
            Dispose(true);
            // 阻止终结器运行
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // 如果已经释放，直接返回
            if (_disposed)
                return;
            // 如果disposing为true，表示由Dispose方法调用，释放托管资源
            if (disposing)
            {
                // 释放托管资源
                Clear();
                // 将托管资源设为null
                itemArray = null;
            }

            // 释放非托管资源

            // 将disposed设为true，表示已经释放
            _disposed = true;
        }

        // 定义终结器，以防止忘记调用Dispose方法
        ~Heap()
        {
            // 调用Dispose(false)只释放非托管资源
            Dispose(false);
        }
    }
}
