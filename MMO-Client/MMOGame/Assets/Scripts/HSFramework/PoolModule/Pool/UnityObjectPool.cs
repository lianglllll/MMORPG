using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HSFramework.PoolModule
{
    [Serializable]
    public class UnityObjectPool : IObjectPool<Object>, IDisposable
    {
        private string poolName = "hello";
        public Object ItemPrefab;
        public int InitialPoolSize = 0;
        private int _curCount = 0;
        public readonly int MaxPoolSize = 500;
        private readonly Func<Object> _objectFactory;
        private readonly Queue<Object> _objects;
        private Action<Object> _enterQueueHandle;
        private Action<Object> _dequeueHandle;

        private Transform _poolRoot;
        public Transform PoolRoot
        {
            get
            {
                if (_poolRoot == null)
                {
                    GameObject obj = GameObject.Find("PoolRoot");
                    if (obj == null)
                    {
                        obj = new GameObject("PoolRoot");
                    }
                    _poolRoot = new GameObject(poolName).transform;
                    _poolRoot.SetParent(obj.transform);
                }

                return _poolRoot;
            }
        }

        private bool _disposed;

        public UnityObjectPool(string poolName, Object itemPrefab, Func<Object> objectFactory, int initialPoolSize = 0, int maxPoolSize = 500, Action<Object> enterQueueHandle = null, Action<Object> dequeueHandle = null)
        {
            this.poolName = poolName;
            _disposed = false;
            ItemPrefab = itemPrefab;
            _objectFactory = objectFactory;
            MaxPoolSize = maxPoolSize;
            InitialPoolSize = initialPoolSize;
            _enterQueueHandle = enterQueueHandle;
            _dequeueHandle = dequeueHandle;
            _objects = new Queue<Object>(MaxPoolSize);
            _curCount = 0;
            for (int i = 0; i < InitialPoolSize; i++)
            {
                var obj = CreateObject();
                if (obj != null)
                    _objects.Enqueue(obj);
            }
        }

        public Object Get()
        {
            Object item = _objects.Count == 0 ? CreateObject() : _objects.Dequeue();
            if (item != null)
                DequeueHandle(item);
            _dequeueHandle?.Invoke(item);
            return item;
        }

        public void Recycle(Object item)
        {
            if (item == null)
                return;
            if (!_objects.Contains(item))
            {
                _enterQueueHandle?.Invoke(item);
                EnqueueHandle(item);
                _objects.Enqueue(item);
            }
        }

        protected Object CreateObject()
        {
            if (_curCount >= MaxPoolSize) return default;
            var newObject = _objectFactory != null ? _objectFactory() : Object.Instantiate(ItemPrefab);
            _curCount++;
            _enterQueueHandle?.Invoke(newObject);
            EnqueueHandle(newObject);
            return newObject;
        }

        public void Cleanup(Func<Object, bool> shouldCleanup)
        {
            int count = _objects.Count;
            for (int i = 0; i < count; i++)
            {
                var obj = _objects.Dequeue();
                if (shouldCleanup(obj))
                {
                    Object.Destroy(obj);
                    _curCount--;
                }
                else
                    _objects.Enqueue(obj);
            }
        }

        public void EnqueueHandle(Object item)
        {
            if (item is GameObject obj)
            {
                obj.SetActive(false);
                obj.transform.SetParent(PoolRoot, true);
            }
        }

        public void DequeueHandle(Object item)
        {
            if (item is GameObject obj)
                obj.SetActive(true);
        }

        public void Dispose()
        {
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                Cleanup(item => true);
                _objects.Clear();
            }

            _disposed = true;
        }

        ~UnityObjectPool()
        {
            Dispose(false);
        }
    }
}
