using System;
using System.Collections.Generic;
using HSFramework.MyDelayedTaskScheduler;
using HSFramework.Tool.Singleton;

namespace HSFramework.PoolModule
{
    //Unity对象池工厂，负责创建对象池、创建对象池中的对象以及回收对象
    public class UnityObjectPoolFactory : SingletonNonMono<UnityObjectPoolFactory>
    {
        //资源加载方法
        public delegate T LoadFuncDel<out T>(string path);
        public LoadFuncDel<UnityEngine.Object> LoadFuncDelegate;
        private readonly Dictionary<string, UnityObjectPool> _pools = new Dictionary<string, UnityObjectPool>();

        //如果创建的Object配置了回收时间，则需要添加到字典中，并添加一个回收事件在指定时间后回收对象
        //如果对象被手动回收了，则需要将对象对应的回收事件移除
        //Dispose时将等待回收的对象都回收掉
        private Dictionary<UnityEngine.Object, AutoRecycleItem> _autoRecycleItems = new Dictionary<UnityEngine.Object, AutoRecycleItem>();
        private AutoRecycleConf _autoRecycleConf = new AutoRecycleConf();
        private UnityObjectPool CreatePool(UnityEngine.Object t, string poolName,
            Func<UnityEngine.Object> objectFactory,
            int initialPoolSize = 0, int maxPoolSize = 200,
            Action<UnityEngine.Object> enqueueHandle = null, Action<UnityEngine.Object> dequeueHandle = null)
        {
            var pool = new UnityObjectPool(poolName, t, objectFactory, initialPoolSize, maxPoolSize, enqueueHandle, dequeueHandle);
            _pools[poolName] = pool;
            return pool;
        }

        public T GetItem<T>(string itemName) where T : UnityEngine.Object
        {
            T result = null;
            float recycleTime = _autoRecycleConf.GetRecycleTime(itemName);
            if (_pools.TryGetValue(itemName, out var pool))
            {
                result = pool.Get() as T;
            }
            else if (LoadFuncDelegate != null)
            {
                result = CreatePool(LoadFuncDelegate(itemName) as T, itemName, null, 0, 500).Get() as T;
            }

            if (result != null && recycleTime > 0)
            {
                AutoRecycleItem autoRecycleItem = ObjectPoolFactory.Instance.GetItem<AutoRecycleItem>();
                autoRecycleItem.DelayTaskToken = DelayedTaskScheduler.Instance.AddDelayedTask(
                    TimerUtil.GetLaterMilliSecondsBySecond(recycleTime), () =>
                    {
                        UnityEngine.Debug.Log($"自动回收了，回收的物体是{itemName}");
                        RecycleItem(itemName, result);
                    });
                _autoRecycleItems.Add(result, autoRecycleItem);
            }

            return result;
        }
        public void RecycleItem(string itemName, UnityEngine.Object objectToReturn)
        {
            if (_pools.TryGetValue(itemName, out var pool))
                pool.Recycle(objectToReturn);
            //如果在等待回收的列表中，则移除，并移除对应的事件
            if (_autoRecycleItems.TryGetValue(objectToReturn, out var autoRecycleItem))
            {
                _autoRecycleItems.Remove(objectToReturn);
                DelayedTaskScheduler.Instance.RemoveDelayedTask(autoRecycleItem.DelayTaskToken);
                ObjectPoolFactory.Instance.RecycleItem(autoRecycleItem);
            }
        }
        public void ClearALL()
        {
            //_pools在不同场景中，使用到的gameobjcet频率可能是不一致的，所有重新获取比较合理一点把。

        }

        private bool _disposed;
        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                //直接取消所有会自动回收的对象的延时任务。
                foreach (KeyValuePair<UnityEngine.Object, AutoRecycleItem> autoRecycleItem in _autoRecycleItems)
                {
                    DelayedTaskScheduler.Instance.RemoveDelayedTask(autoRecycleItem.Value.DelayTaskToken);
                    ObjectPoolFactory.Instance.RecycleItem(autoRecycleItem.Value);
                }

                _autoRecycleItems.Clear();
                _autoRecycleConf.Dispose();
                _autoRecycleConf = null;

                foreach (var pool in _pools.Values)
                {
                    pool?.Dispose();
                }

                _pools.Clear();
            }

            // 确保调用父类的 Dispose 方法
            base.Dispose(disposing);
            _disposed = true;

        }
        ~UnityObjectPoolFactory()
        {
            Dispose(false);
        }
    }
}
