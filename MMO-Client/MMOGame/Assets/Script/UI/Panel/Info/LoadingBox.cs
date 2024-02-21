using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingBox : MonoBehaviour
{
    private TextMeshProUGUI textMeshProUGUI;

    private void Awake()
    {
        textMeshProUGUI = transform.Find("ContentBox/Text").GetComponent<TextMeshProUGUI>();
    }

    public void Show(string value)
    {
        textMeshProUGUI.text = value;
    }


}
