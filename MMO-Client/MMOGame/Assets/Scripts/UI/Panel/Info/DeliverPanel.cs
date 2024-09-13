using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliverPanel : MonoBehaviour
{
    //面板的弹出效果
    private PanelAnim panelAnim;


    private void Awake()
    {
        panelAnim = transform.GetComponent<PanelAnim>();
        //transform.localScale = Vector3.one * 0;
    }

    private void Start()
    {
    }


    public void Show()
    {
        panelAnim.Show(gameObject);
    }

    public void Hide(Action hideEndAction)
    {
        panelAnim.Hide(gameObject,hideEndAction);
    }

}

