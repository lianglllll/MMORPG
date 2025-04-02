using System;

namespace HSFramework.PoolModule
{
    // 对象池接口
    public interface IObjectPool<T>
    {
        // 获取obj
        T Get();
        // 回收obj
        void Recycle(T item);
        // 清理整个对象池
        void Cleanup(Func<T, bool> shouldCleanup);

        // 入对象池时的处理
        void EnqueueHandle(T item);
        // 出对象池时的处理
        void DequeueHandle(T item);
    }
}
