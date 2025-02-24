using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace HSFramework.PoolModule
{
    /// <summary>
    /// 多线程版本的对象池, ConcurrentBag内部自己实现了线程安全
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ThreadObjectPool<T> : IObjectPool<T>, IDisposable where T : new()
    {
        private ConcurrentDictionary<T, byte> _objects;
        private Func<T> _objectFactory;
        private int InitialPoolSize = 0;
        private readonly int MaxPoolSize = 200;
        private int _curCount = 0;
        private bool _disposed;

        public ThreadObjectPool(Func<T> objectFactory, int maxPoolSize = 200, int initialPoolSize = 0)
        {
            _disposed = false;
            _objectFactory = objectFactory;
            _objects = new ConcurrentDictionary<T, byte>();
            MaxPoolSize = maxPoolSize;
            InitialPoolSize = initialPoolSize;
            _curCount = 0;
            for (int i = 0; i < InitialPoolSize; i++)
            {
                var obj = CreateObject();
                if (obj != null)
                    _objects.TryAdd(obj, 0);
            }
        }

        public T Get()
        {
            T item;
            if (_objects.Count == 0)
                item = CreateObject();
            else
            {
                var enumerator = _objects.GetEnumerator();
                enumerator.MoveNext();
                item = enumerator.Current.Key;
                DequeueHandle(item);
            }

            return item;
        }

        public void Recycle(T item)
        {
            if (item == null)
                return;
            if (!_objects.ContainsKey(item))
            {
                EnqueueHandle(item);
                _objects.TryAdd(item, 0);
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

        List<T> _tempList = new List<T>();

        public void Cleanup(Func<T, bool> shouldCleanup)
        {
            _tempList.Clear();
            foreach (KeyValuePair<T, byte> keyValuePair in _objects)
            {
                if (shouldCleanup(keyValuePair.Key))
                {
                    _tempList.Add(keyValuePair.Key);
                }
            }

            foreach (T t in _tempList)
            {
                if (_objects.TryRemove(t, out _))
                    _curCount--;
            }

            _tempList.Clear();
        }

        public void EnqueueHandle(T item)
        {
        }

        public void DequeueHandle(T item)
        {
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
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

            _curCount--;
            _disposed = true;
        }

        ~ThreadObjectPool()
        {
            Dispose(false);
        }
    }
}
