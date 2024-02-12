using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomer : MonoBehaviour
{
    private Vector3 _pos;
    public float Radius = 5.0f;
    public float FirePower = 10f;
    public float UpwardPower = 5f;
    public ForceMode BoomForceMode = ForceMode.Impulse;

    private void Start()
    {
        _pos = transform.position;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryBoom();
        }
    }

    private void TryBoom()
    {
        Collider[] colliders = Physics.OverlapSphere(_pos, Radius);
        foreach(var col in colliders)
        {
            Rigidbody rig = col.GetComponent<Rigidbody>();
            if (rig!= null && col.CompareTag("Timer"))
            {
                rig.AddExplosionForce(FirePower, _pos, Radius, UpwardPower, BoomForceMode);
            }
            else
            {
                Debug.Log($"{col.name}没有Rigidbody");
            }
        }
    }
}
