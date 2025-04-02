using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HSFramework.PoolModule
{
    [Serializable]
    public class UnityObjectPool : IObjectPool<Object>, IDisposable
    {
        private string                  m_poolName = "不过是些许风霜罢了";
        public Object                   m_itemPrefab;
        public int                      m_initialPoolSize = 0;
        private int                     m_curCount = 0;
        public readonly int             m_maxPoolSize = 500;
        private readonly Func<Object>   m_objectFactory;
        private readonly Queue<Object>  m_objects;
        private Action<Object>          m_enterQueueHandle;
        private Action<Object>          m_dequeueHandle;
        private Transform               m_poolRoot;
        private bool                    m_disposed;

        public Transform PoolRoot
        {
            get
            {
                if (m_poolRoot == null)
                {
                    GameObject obj = GameObject.Find("PoolRoot");
                    if (obj == null)
                    {
                        obj = new GameObject("PoolRoot");
                    }
                    m_poolRoot = new GameObject(m_poolName).transform;
                    m_poolRoot.SetParent(obj.transform);
                }
                return m_poolRoot;
            }
        }


        public UnityObjectPool(
            string poolName,
            Object itemPrefab,
            Func<Object> objectFactory,
            int initialPoolSize = 0,
            int maxPoolSize = 500,
            Action<Object> enterQueueHandle = null,
            Action<Object> dequeueHandle = null)
        {
            this.m_poolName = poolName;
            m_disposed = false;
            m_itemPrefab = itemPrefab;
            m_objectFactory = objectFactory;
            m_maxPoolSize = maxPoolSize;
            m_initialPoolSize = initialPoolSize;
            m_enterQueueHandle = enterQueueHandle;
            m_dequeueHandle = dequeueHandle;
            m_objects = new Queue<Object>(m_maxPoolSize);
            m_curCount = 0;
            for (int i = 0; i < m_initialPoolSize; i++)
            {
                var obj = _CreateObject();
                if (obj != null)
                    m_objects.Enqueue(obj);
            }
        }
        ~UnityObjectPool()
        {
            Dispose(false);
        }
        public void Dispose()
        {
        }
        protected virtual void Dispose(bool disposing)
        {
            if (m_disposed)
                return;
            if (disposing)
            {
                Cleanup(item => true);
                m_objects.Clear();
            }

            m_disposed = true;
        }

        public Object Get()
        {
            Object item = default;
            if(m_objects.Count > 0)
            {
                item = m_objects.Dequeue();
            }
            else
            {
                item = _CreateObject();
            }
            if (item != null) {
                DequeueHandle(item);
            }
            m_dequeueHandle?.Invoke(item);
            return item;
        }
        public void Recycle(Object item)
        {
            if (item == null)
            {
                goto End;
            }
            if (!m_objects.Contains(item))
            {
                m_enterQueueHandle?.Invoke(item);
                EnqueueHandle(item);
                m_objects.Enqueue(item);
            }
        End:
            return;
        }
        public void Cleanup(Func<Object, bool> shouldCleanup)
        {
            int count = m_objects.Count;
            for (int i = 0; i < count; i++)
            {
                var obj = m_objects.Dequeue();
                if (shouldCleanup(obj))
                {
                    Object.Destroy(obj);
                    m_curCount--;
                }
                else
                    m_objects.Enqueue(obj);
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


        private Object _CreateObject()
        {
            if (m_curCount >= m_maxPoolSize) return default;
            var newObject = m_objectFactory != null ? m_objectFactory() : Object.Instantiate(m_itemPrefab);
            m_curCount++;
            m_enterQueueHandle?.Invoke(newObject);
            EnqueueHandle(newObject);
            return newObject;
        }

    }
}
