using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cinemachine;

namespace MirzaBeig.LightningVFX
{
    public class CustomImpulse : MonoBehaviour
    {
        CinemachineImpulseSource source;

        void Start()
        {

        }

        void OnEnable()
        {
            if (!source)
            {
                source = GetComponent<CinemachineImpulseSource>();
            }

            source.GenerateImpulse();
        }

        void Update()
        {

        }
    }
}
