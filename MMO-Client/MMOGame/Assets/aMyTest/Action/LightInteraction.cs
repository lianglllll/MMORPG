using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightInteraction : InteractionBehaviour
{
    public int id;
    private Light _light;

    private void Awake()
    {
        _light = GetComponentInChildren<Light>();
        _light.enabled = false;
    }

    protected override void Start()
    {
        id = InteractionManager.Instance.AddInteraciton(this);           
    }


    protected override void Interaction()
    {
        if (_light == null) return;
        _light.enabled = !_light.enabled;

    }
}
