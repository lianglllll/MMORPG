using HSFramework.Tool.Singleton;
using System;
using System.Collections.Generic;

namespace HSFramework.PoolModule
{
    /// <summary>
    /// 通用对象池工厂，维护多个对象池
    /// </summary>
    public class ObjectPoolFactory : SingletonNonMono<ObjectPoolFactory>
    {
        private readonly Dictionary<System.Type, object> _pools = new Dictionary<Type, object>();
        private const int DefaultPoolSize = 2;
        private const int DefaultPoolMaxSize = 500;

        public ObjectPoolFactory()
        {
            _disposed = false;
        }

        private ObjectPool<T> GetPool<T>(Func<T> objectGenerator = null, int poolSize = DefaultPoolSize) where T : new()
        {
            var type = typeof(T);
            if (!_pools.TryGetValue(type, out var pool))
            {
                pool = new ObjectPool<T>(objectGenerator, poolSize, DefaultPoolMaxSize);
                _pools.Add(type, pool);
            }

            return pool as ObjectPool<T>;
        }


        public T GetItem<T>() where T : new()
        {
            //使用列表时一定要注意清空列表，防止残留前面的数据导致Bug
            return GetPool<T>().Get();
        }
        public void RecycleItem<T>(T item) where T : new()
        {
            GetPool<T>().Recycle(item);
        }

        private bool _disposed = false;
        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                foreach (var pool in _pools)
                {
                    (pool.Value as IDisposable)?.Dispose();
                }

                _pools.Clear();
            }

            // 确保调用父类的 Dispose 方法
            base.Dispose(disposing);

            _disposed = true;
        }
        ~ObjectPoolFactory()
        {
            Dispose(false);
        }

    }
}
