using HSFramework.MySingleton;
using System;
using System.Collections.Generic;

namespace HSFramework.PoolModule
{
    /// <summary>
    /// 通用对象池工厂，维护多个对象池
    /// </summary>
    public class ObjectPoolFactory : SingletonNonMono<ObjectPoolFactory>
    {
        private readonly Dictionary<System.Type, object> m_pools    = new();
        private const int m_defaultPoolSize                         = 2;
        private const int m_defaultPoolMaxSize                      = 500;
        private bool m_disposed                                     = false;

        public ObjectPoolFactory()
        {
            m_disposed = false;
        }
        ~ObjectPoolFactory()
        {
            Dispose(false);
        }
        protected override void Dispose(bool disposing)
        {
            if (m_disposed)
                return;
            m_disposed = true;

            if (disposing)
            {
                foreach (var pool in m_pools)
                {
                    (pool.Value as IDisposable)?.Dispose();
                }
                m_pools.Clear();
            }

            // 确保调用父类的 Dispose 方法
            base.Dispose(disposing);
        }

        private ObjectPool<T> _GetPool<T>(Func<T> objectGenerator = null, int poolSize = m_defaultPoolSize) where T : new()
        {
            var type = typeof(T);
            if (!m_pools.TryGetValue(type, out var pool))
            {
                pool = new ObjectPool<T>(objectGenerator, poolSize, m_defaultPoolMaxSize);
                m_pools.Add(type, pool);
            }

            return pool as ObjectPool<T>;
        }

        public T GetItem<T>() where T : new()
        {
            // 使用列表时一定要注意清空列表，防止残留前面的数据导致Bug
            return _GetPool<T>().Get();
        }
        public void RecycleItem<T>(T item) where T : new()
        {
            _GetPool<T>().Recycle(item);
        }
    }
}
