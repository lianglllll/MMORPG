using System;
using System.Collections;
using System.Collections.Generic;

namespace HSFramework.PoolModule
{
    // 通用的对象池实现(只存放某个对象)
    public class ObjectPool<T> : IObjectPool<T>, IDisposable where T : new()
    {
        private bool            m_disposed;
        private Queue<T>        m_objects;                  // 存放我们已经存在的对象
        private Func<T>         m_objectFactory;            // 创建对象的方法
        private int             m_initialPoolSize   = 0;    // 初始对象数量
        private int             m_curCount          = 0;    // 当前对象数量
        private readonly int    m_maxPoolSize       = 200;  // 最大对象数量

        public ObjectPool(Func<T> objectFactory, int initialPoolSize = 0, int maxPoolSize = 200)
        {
            m_disposed = false;
            m_objectFactory = objectFactory;
            m_objects = new Queue<T>();
            m_maxPoolSize = maxPoolSize;
            m_initialPoolSize = initialPoolSize;
            m_curCount = 0;
            for (int i = 0; i < m_initialPoolSize; i++)
            {
                var obj = _CreateObject();
                if (obj != null) {
                    m_objects.Enqueue(obj);
                }
            }
        }
        ~ObjectPool()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            if (m_disposed)
                return;
            if (disposing)
            {
                if (m_objects != null)
                {
                    m_objects.Clear();
                    m_objects = null;
                }

                m_objectFactory = null;
            }

            m_curCount = 0;
            m_disposed = true;
        }

        public T Get()
        {
            T result = default;
            if(m_objects.Count > 0)
            {
                result = m_objects.Dequeue();
            }
            else
            {
                result = _CreateObject();
            }
            if (result != null)
                DequeueHandle(result);
            return result;
        }
        public void Recycle(T item)
        {
            if (item == null)
            {
                goto End;
            }
                
            if (!m_objects.Contains(item))
            {
                EnqueueHandle(item);
                m_objects.Enqueue(item);
            }

        End:
            return;
        }
        public void Cleanup(Func<T, bool> shouldCleanupFunc)
        {
            int count = m_objects.Count;
            for (int i = 0; i < count; i++)
            {
                T item = m_objects.Dequeue();
                if (!shouldCleanupFunc(item))
                    m_objects.Enqueue(item);
                else
                    m_curCount--;
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

        private T _CreateObject()
        {
            T newObject = default;

            if (m_curCount >= m_maxPoolSize)
            {
                goto End;
            }

            if(m_objectFactory != null)
            {
                newObject = m_objectFactory.Invoke();
            }
            else
            {
                newObject = new T();
            }

            m_curCount++;
            EnqueueHandle(newObject);

        End:
            return newObject;
        }
    }
}
