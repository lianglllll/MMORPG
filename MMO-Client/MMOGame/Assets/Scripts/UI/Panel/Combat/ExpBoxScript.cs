using GameClient.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpBoxScript : MonoBehaviour
{
    private Text expText;
    private Slider expSlider;

    private Actor _actor;

    private void Awake()
    {
        expText = transform.Find("ExpSlider/ExpText").GetComponent<Text>();
        expSlider = transform.Find("ExpSlider/Slider").GetComponent<Slider>();
    }

    public void Init(Actor actor)
    {
        this._actor = actor;
        RefrashUI();
    }

    public void RefrashUI()
    {
        if (_actor == null) return;
        var def = DataManager.Instance.levelDefineDict[_actor.Level];
        if (def == null) return;
        expText.text = "" + _actor.Exp + "/" + def.ExpLimit;
        var proportion = (_actor.Exp*1.0f / def.ExpLimit);
        expSlider.value = proportion;
    }
}
