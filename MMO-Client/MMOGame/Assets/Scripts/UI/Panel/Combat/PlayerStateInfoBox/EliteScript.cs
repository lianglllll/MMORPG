using UnityEngine;
using UnityEngine.UI;
using GameClient.Entities;
using System.Collections.Generic;

public class EliteScript : MonoBehaviour
{
    private Slider m_HPSlider;
    private Image m_hpFillImage;
    private Slider m_MPSlider;
    private Text m_PlayerName;
    private BuffGroupScript m_buffGroup;
    private Actor actor;                //UI显示要的目标
    private List<Color> hpColors;

    private void Awake()
    {
        m_PlayerName = transform.Find("Name").GetComponent<Text>();
        m_HPSlider = transform.Find("HPBar/Slider").GetComponent<Slider>();
        m_hpFillImage = transform.Find("HPBar/Slider/Fill Area/Fill").GetComponent<Image>();
        m_MPSlider = transform.Find("MPBar/Slider").GetComponent<Slider>();
        m_buffGroup = transform.Find("BuffGroup").GetComponent<BuffGroupScript>();
    }

    public void Init(Actor actor)
    {
        if (actor == null) {
            this.actor = null;
            return;
        }
        if (this.actor == actor) return;

        this.actor = actor;
        m_buffGroup.SetOwner(actor);

        ColorUtility.TryParseHtmlString("#AEAEAE", out var color1);
        ColorUtility.TryParseHtmlString("#1FBC1E", out var color2);
        ColorUtility.TryParseHtmlString("#BC2F1E", out var color3);
        hpColors = new();
        hpColors.Add(color1);
        hpColors.Add(color2);
        hpColors.Add(color3);

        RefreshUI();
    }

    public void RefreshUI()
    {
        if (actor == null) return;
        m_PlayerName.text = actor.ActorName;
        float hp = (actor.Hp * 1.0f) / actor.MaxHp;
        float mp = (actor.Mp * 1.0f) / actor.MaxMp;
        if(0.1f < hp && hp < 1)
        {
            m_hpFillImage.color = hpColors[1];
        }
        else if(hp < 0.1f)
        {
            m_hpFillImage.color = hpColors[2];
        }
        else
        {
            m_hpFillImage.color = hpColors[0];
        }
        m_HPSlider.value = hp;
        m_MPSlider.value = mp;
    }
}
