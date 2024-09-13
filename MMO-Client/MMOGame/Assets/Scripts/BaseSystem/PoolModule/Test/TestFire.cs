using System;
using UnityEngine;

namespace BaseSystem.PoolModule
{
    public class TestFire : MonoBehaviour
    {
        private Transform _transform;
        private Camera _mainCamera;
        public float _fireInterval = 0.1f;
        private float _fireTimer = 0f;
        [Header("是否使用对象池？")]
        public bool UsePool = false;
        void Awake()
        {
            _transform = transform;
            _mainCamera = Camera.main;
            //这里应该放在一个更合适的地方，这里只是为了演示所以放在这里
            UnityObjectPoolFactory.Instance.LoadFuncDelegate = PoolAssetLoad.LoadAssetByResource<UnityEngine.Object>;
        }
        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                _fireTimer -= Time.deltaTime;
                if (_fireTimer <= 0)
                {
                    GameObject bulletObj = null;
                    if (UsePool)
                        bulletObj = UnityObjectPoolFactory.Instance.GetItem<GameObject>(TestBullet.BulletName);
                    else
                    {
                        var prefabObj = PoolAssetLoad.LoadAssetByResource<UnityEngine.GameObject>(TestBullet.BulletName);
                        bulletObj = Instantiate(prefabObj);
                    }

                    if (bulletObj != null)
                    {
                        _fireTimer = _fireInterval;
                        var position = _transform.position;
                        bulletObj.transform.position = position;
                        var screenPoint = _mainCamera.WorldToScreenPoint(Input.mousePosition);
                        var dir =  Input.mousePosition - _mainCamera.WorldToScreenPoint(position);
                        dir.z = 0;
                        dir.Normalize();
                        TestBullet testBullet = bulletObj.GetComponent<TestBullet>();
                        testBullet.UsePool(UsePool);
                        testBullet.SetMoveDirection(dir);
                    }
                }
            }
        }
    }
}
