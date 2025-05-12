using GameClient.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitBillboard : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI nameText;

    private Actor m_owner;
    private bool m_isInit;

    public void Init(Actor actor)
    {
        m_owner = actor;
        slider.value = (m_owner.Hp * 1.0f) / m_owner.MaxHp;
        nameText.text = m_owner.ActorName;
        m_isInit = true;
    }

    public void UpdateHpBar()
    {
        if (!m_isInit) return;
        slider.value = (m_owner.Hp * 1.0f) / m_owner.MaxHp;
    }
}
