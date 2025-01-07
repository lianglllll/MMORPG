using System;


namespace BaseSystem.Tool.Singleton
{
    public abstract class SingletonNonMono<T> : IDisposable where T : class, new()
    {
        private static T _instance;
        private static object _lock = new object();
        private bool _disposed = false;

        protected SingletonNonMono() { } // 防止外部直接实例化

        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance ??= new T();
                    }
                }
                return _instance;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // 这里放置父类需要清理的托管资源
                }

                // 清理父类的非托管资源
                lock (_lock)
                {
                    _instance = null;
                }
                _disposed = true;
            }
        }
        ~SingletonNonMono()
        {
            Dispose(false);
        }

    }

}
