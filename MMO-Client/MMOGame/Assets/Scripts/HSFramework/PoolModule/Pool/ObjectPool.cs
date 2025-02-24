using System;
using System.Collections;
using System.Collections.Generic;

namespace HSFramework.PoolModule
{
    //通用的对象池实现(只存放某个对象)
    public class ObjectPool<T> : IObjectPool<T>, IDisposable where T : new()
    {
        private bool _disposed;
        private Queue<T> _objects;                  //存放我们已经存在的对象
        private Func<T> _objectFactory;             //创建对象的方法
        private int InitialPoolSize = 0;            //初始对象数量
        private int _curCount = 0;                  //当前对象数量
        private readonly int MaxPoolSize = 200;     //最大对象数量

        public ObjectPool(Func<T> objectFactory, int initialPoolSize = 0, int maxPoolSize = 200)
        {
            _disposed = false;
            _objectFactory = objectFactory;
            _objects = new Queue<T>();
            MaxPoolSize = maxPoolSize;
            InitialPoolSize = initialPoolSize;
            _curCount = 0;
            for (int i = 0; i < InitialPoolSize; i++)
            {
                var obj = CreateObject();
                if (obj != null)
                    _objects.Enqueue(obj);
            }
        }
        private T CreateObject()
        {
            if (_curCount >= MaxPoolSize) return default;

            var newObject = _objectFactory != null ? _objectFactory() : new T();
            _curCount++;
            EnqueueHandle(newObject);
            return newObject;
        }


        public T Get()
        {
            T item = _objects.Count == 0 ? CreateObject() : _objects.Dequeue();
            if (item != null)
                DequeueHandle(item);
            return item;
        }
        public void Recycle(T item)
        {
            if (item == null)
                return;
            if (!_objects.Contains(item))
            {
                EnqueueHandle(item);
                _objects.Enqueue(item);
            }
        }
        public void Cleanup(Func<T, bool> shouldCleanupFunc)
        {
            int count = _objects.Count;
            for (int i = 0; i < count; i++)
            {
                T item = _objects.Dequeue();
                if (!shouldCleanupFunc(item))
                    _objects.Enqueue(item);
                else
                    _curCount--;
            }
        }
        public void EnqueueHandle(T item)
        {
            if(item is IObjectPoolItem iPoolItem)
                iPoolItem.OnRecycleHandle();
            if(item is IList list)
                list.Clear();
            else if(item is IDictionary dictionary)
                dictionary.Clear();
        }
        public void DequeueHandle(T item)
        {
            if(item is IObjectPoolItem iPoolItem)
                iPoolItem.OnGetHandle();
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                if (_objects != null)
                {
                    _objects.Clear();
                    _objects = null;
                }

                _objectFactory = null;
            }

            _curCount = 0;
            _disposed = true;
        }
        ~ObjectPool()
        {
            Dispose(false);
        }
    }
}
