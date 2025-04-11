using System;
using System.Collections.Generic;
using HSFramework.MyDelayedTaskScheduler;
using HSFramework.MySingleton;

namespace HSFramework.PoolModule
{
    // Unity对象池工厂，负责创建对象池、创建对象池中的对象以及回收对象
    // 如果创建的Object配置了回收时间，则需要添加到字典中，并添加一个回收事件在指定时间后回收对象
    // 如果对象被手动回收了，则需要将对象对应的回收事件移除
    // Dispose时将等待回收的对象都回收掉
    public class UnityObjectPoolFactory : SingletonNonMono<UnityObjectPoolFactory>
    {
        public delegate T LoadResFuncDelegate<out T>(string path);                      // 协变
        private LoadResFuncDelegate<UnityEngine.Object>             m_loadResFunc;     // 资源加载方法
        private bool                                                m_disposed;
        private readonly Dictionary<string, UnityObjectPool>        m_pools = new();
        private Dictionary<UnityEngine.Object, AutoRecycleItem>     m_autoRecycleItems = new();
        private AutoRecycleConf                                     m_autoRecycleConf = new AutoRecycleConf();

        public bool Init(LoadResFuncDelegate<UnityEngine.Object> loadResFunc)
        {
            m_loadResFunc = loadResFunc;
            return true;
        }
        ~UnityObjectPoolFactory()
        {
            Dispose(false);
        }
        protected override void Dispose(bool disposing)
        {
            if (m_disposed) return;
            if (disposing)
            {
                // 直接取消所有会自动回收的对象的延时任务。
                foreach (KeyValuePair<UnityEngine.Object, AutoRecycleItem> autoRecycleItem in m_autoRecycleItems)
                {
                    DelayedTaskScheduler.Instance.RemoveDelayedTask(autoRecycleItem.Value.DelayTaskToken);
                    ObjectPoolFactory.Instance.RecycleItem(autoRecycleItem.Value);
                }

                m_autoRecycleItems.Clear();
                m_autoRecycleConf.Dispose();
                m_autoRecycleConf = null;

                foreach (var pool in m_pools.Values)
                {
                    pool?.Dispose();
                }

                m_pools.Clear();
            }

            // 确保调用父类的 Dispose 方法
            base.Dispose(disposing);
            m_disposed = true;
        }

        public T GetItem<T>(string itemName) where T : UnityEngine.Object
        {
            T result = null;

            if (m_pools.TryGetValue(itemName, out var pool))
            {
                result = pool.Get() as T;
            }
            else
            {
                result = _CreatePool(itemName, m_loadResFunc(itemName) as T, null, 0, 500).Get() as T;
            }


            float autoRecycleTime = m_autoRecycleConf.GetRecycleTime(itemName);
            if (result != null && autoRecycleTime > 0)
            {
                AutoRecycleItem autoRecycleItem = ObjectPoolFactory.Instance.GetItem<AutoRecycleItem>();
                autoRecycleItem.DelayTaskToken = DelayedTaskScheduler.Instance.AddDelayedTask(
                    TimerUtil.GetLaterMilliSecondsBySecond(autoRecycleTime), () =>
                    {
                        UnityEngine.Debug.Log($"自动回收了，回收的物体是{itemName}");
                        RecycleItem(itemName, result);
                    });
                m_autoRecycleItems.Add(result, autoRecycleItem);
            }

            return result;
        }
        public void RecycleItem(string itemName, UnityEngine.Object objectToReturn)
        {
            if (m_pools.TryGetValue(itemName, out var pool))
                pool.Recycle(objectToReturn);
            // 如果在等待回收的列表中，则移除，并移除对应的事件
            if (m_autoRecycleItems.TryGetValue(objectToReturn, out var autoRecycleItem))
            {
                m_autoRecycleItems.Remove(objectToReturn);
                DelayedTaskScheduler.Instance.RemoveDelayedTask(autoRecycleItem.DelayTaskToken);
                ObjectPoolFactory.Instance.RecycleItem(autoRecycleItem);
            }
        }
        public void ClearALL()
        {
            //_pools在不同场景中，使用到的gameobjcet频率可能是不一致的，所有重新获取比较合理一点把。

        }


        private UnityObjectPool _CreatePool(
            string poolName,
            UnityEngine.Object prefab,
            Func<UnityEngine.Object> objectFactory,
            int initialPoolSize = 0,
            int maxPoolSize = 200,
            Action<UnityEngine.Object> enqueueHandle = null,
            Action<UnityEngine.Object> dequeueHandle = null)
        {
            var pool = new UnityObjectPool(poolName, prefab, objectFactory,
                initialPoolSize, maxPoolSize, enqueueHandle, dequeueHandle);
            m_pools[poolName] = pool;
            return pool;
        }
    }
}
