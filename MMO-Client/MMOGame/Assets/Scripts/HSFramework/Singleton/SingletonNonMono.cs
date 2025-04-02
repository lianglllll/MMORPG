using System;
namespace HSFramework.MySingleton
{
    public abstract class SingletonNonMono<T> : IDisposable where T : class, new()
    {
        private static T _instance;
        private static object m_lock = new object();
        private bool m_disposed = false;

        protected SingletonNonMono() { } // 防止外部直接实例化
        ~SingletonNonMono()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            if (m_disposed) return;
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            // disposing 参数的意义：
            // true：由用户代码调用 Dispose() 触发，需释放托管和非托管资源。
            // false：由终结器触发，只能释放非托管资源​（此时托管资源可能已被 GC 回收）。

            if (m_disposed) return;
            m_disposed = true;

            if (disposing)
            {
                // 释放托管资源（如其他实现了 IDisposable 的对象）
            }
            // 释放非托管资源（如文件句柄）
            lock (m_lock)
            {
                _instance = null;
            }
        }

        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;
                lock (m_lock)
                {
                    if (_instance == null)
                    {
                        _instance ??= new T();
                    }
                }
                return _instance;
            }
        }
    }
}
