using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TaskListDescItem : MonoBehaviour
{
    private TextMeshProUGUI m_taskTitleText;
    private void Awake()
    {
        m_taskTitleText = transform.Find("TaskTitleText").GetComponent<TextMeshProUGUI>();
    }
    public void Init(string title)
    {
        m_taskTitleText.text = title;
    }
}
