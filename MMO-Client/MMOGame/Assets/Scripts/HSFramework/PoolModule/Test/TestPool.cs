using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HSFramework.PoolModule
{
    public class TestPool : MonoBehaviour
    {
        private GameObject PoolItemObj;
        private GameObject AutoRecyclePoolItemObj1;
        private GameObject AutoRecyclePoolItemObj2;
        private string PoolItemName = "TestPool";
        private string AutoRecyclePoolItemObjName1 = "TestPool1";
        private string AutoRecyclePoolItemObjName2 = "TestPool2";

        private void Awake()
        {
            UnityObjectPoolFactory.Instance.LoadFuncDelegate = PoolAssetLoad.LoadAssetByResource<UnityEngine.Object>;
            // UnityObjectPoolFactory.Instance.LoadFunc = TestAssetLoad.LoadAsset;
        }

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.C))
            {
                UnityObjectPoolFactory.Instance.GetItem<GameObject>(AutoRecyclePoolItemObjName1).
                    transform.position = new Vector3(Random.Range(-10, 10), Random.Range(-5, 5), Random.Range(-5, 5));
            }
        }

        [ContextMenu("读取普通物体")]
        public void LoadGameObject()
        {
            if (PoolItemObj != null)
            {
                UnityObjectPoolFactory.Instance.RecycleItem(PoolItemName, PoolItemObj);
                PoolItemObj = null;
            }

            PoolItemObj = UnityObjectPoolFactory.Instance.GetItem<GameObject>(PoolItemName);
        }

        [ContextMenu("读取自动回收的物体")]
        public void LoadAutoRecycleGameObject()
        {
            RecycleAutoRecycleGameObject1();
            RecycleAutoRecycleGameObject2();

            AutoRecyclePoolItemObj1 = UnityObjectPoolFactory.Instance.GetItem<GameObject>(AutoRecyclePoolItemObjName1);
            AutoRecyclePoolItemObj2 = UnityObjectPoolFactory.Instance.GetItem<GameObject>(AutoRecyclePoolItemObjName2);
        }


        [ContextMenu("手动回收自动回收的物体1")]
        public void RecycleAutoRecycleGameObject1()
        {
            if (AutoRecyclePoolItemObj1 != null)
            {
                UnityObjectPoolFactory.Instance.RecycleItem(AutoRecyclePoolItemObjName1, AutoRecyclePoolItemObj1);
                AutoRecyclePoolItemObj1 = null;
            }
        }

        [ContextMenu("手动回收自动回收的物体2")]
        public void RecycleAutoRecycleGameObject2()
        {
            if (AutoRecyclePoolItemObj2 != null)
            {
                UnityObjectPoolFactory.Instance.RecycleItem(AutoRecyclePoolItemObjName2, AutoRecyclePoolItemObj2);
                AutoRecyclePoolItemObj2 = null;
            }
        }
    }
}
