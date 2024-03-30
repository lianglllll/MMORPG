using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemMenuOption : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{

    public TextMeshProUGUI textPro;

    public Color normalColor = Color.white; // 正常状态下的颜色
    public Color hoverColor = Color.red; // 鼠标滑过时的颜色

    private Image image;


    public void OnPointerClick(PointerEventData eventData)
    {
        ItemMenu.Instance.optionCallback?.Invoke(textPro?.text);
        ItemMenu.Hide();
    }

    void Start()
    {
        image = GetComponent<Image>();
        if (image != null)
        {
            image.color = normalColor; // 将初始颜色设置为正常状态下的颜色
        }
    }

    // 当鼠标进入游戏对象时触发
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (image != null)
        {
            image.color = hoverColor; // 将颜色设置为鼠标滑过时的颜色
        }
    }

    // 当鼠标离开游戏对象时触发
    public void OnPointerExit(PointerEventData eventData)
    {
        if (image != null)
        {
            image.color = normalColor; // 将颜色恢复为正常状态下的颜色
        }
    }

    internal void SetLabel(string value)
    {
        textPro.text = value;
    }

    private void LateUpdate()
    {
        
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RectTransform panelRectTransform = ItemMenu.Instance.transform.GetComponent<RectTransform>();

            Vector2 localMousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(panelRectTransform, Input.mousePosition, Camera.main, out localMousePosition);

            if (panelRectTransform.rect.Contains(localMousePosition))
            {
                // 鼠标在Panel范围内
            }
            else
            {
                // 鼠标不在Panel范围内
                // 延迟隐藏，防止影响正常点击
                StartCoroutine(DelayedHide());
            }
        }

        IEnumerator DelayedHide()
        {
            yield return new WaitForSeconds(0.1f);
            ItemMenu.Hide();
        }

    }

}
