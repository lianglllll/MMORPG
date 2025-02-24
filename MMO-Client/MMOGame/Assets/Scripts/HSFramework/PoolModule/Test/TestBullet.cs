using System;
using UnityEngine;

namespace HSFramework.PoolModule
{
    public class TestBullet : MonoBehaviour
    {
        public float MoveSpeed = 1f;
        private Vector3 moveDir = Vector3.zero;
        private Transform _transform;
        public static string BulletName = "TestBullet";
        private bool _usePool;

        private void Awake()
        {
            _transform = transform;
        }

        public void SetMoveDirection(Vector3 dir)
        {
            moveDir = dir;
        }
        public void UsePool(bool usePool)
        {
            _usePool = usePool;
        }

        private void FixedUpdate()
        {
            _transform.position += moveDir * MoveSpeed * Time.fixedDeltaTime;
        }
        
        private void OnBecameInvisible()
        {
            if(_usePool)
                UnityObjectPoolFactory.Instance.RecycleItem(BulletName, gameObject);
            else
                Destroy(gameObject);
        }
    }
}
