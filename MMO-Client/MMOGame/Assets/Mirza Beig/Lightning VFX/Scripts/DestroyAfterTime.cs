using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MirzaBeig.LightningVFX
{
    public class DestroyAfterTime : MonoBehaviour
    {
        public float time = 2.0f;

        void Start()
        {
            Destroy(gameObject, time);
        }
    }
}
