using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VocationItem : MonoBehaviour
{
    private Text text;
    private CanvasGroup Select;
    private Button btn;
    private float selectDuration = 0.5f;
    private bool isSelected;

    private UnitDefine define;
    private CreateRolePanelScript createRolePanelScript;

    public int JobId => define.TID;
    public string Tname => define.Name;

    private void Awake()
    {
        text = transform.Find("Text").GetComponent<Text>();
        Select = transform.Find("Select").GetComponent<CanvasGroup>();
        btn = GetComponent<Button>();
    }

    void Start()
    {
        btn.onClick.AddListener(OnBtn);
        Select.alpha = 0;
        Select.gameObject.SetActive(false);
        isSelected = false;
    }

    public void Init(CreateRolePanelScript createRolePanelScript, UnitDefine define)
    {
        this.define = define;
        this.createRolePanelScript = createRolePanelScript;
        text.text = define.Name;
    }

    public void OnBtn()
    {
        if (isSelected) return;
        createRolePanelScript.OnSelectBtn(this);
    }

    public void SelectedEffect()
    {
        isSelected = true;
        Select.gameObject.SetActive(true);
        Select.DOFade(1,selectDuration);
    }

    public void RestoreEffect()
    {
        Select.DOFade(0, selectDuration).OnComplete(() => {
            Select.gameObject.SetActive(false);
            isSelected = false;
        });

    }

}
