using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemIOInfoItem : MonoBehaviour
{
    private TextMeshProUGUI text;

    private void Awake()
    {
        text = transform.Find("Text").GetComponent<TextMeshProUGUI>();
    }

    public void ShowMsg(string msg)
    {
        text.text = msg;
    }
}
