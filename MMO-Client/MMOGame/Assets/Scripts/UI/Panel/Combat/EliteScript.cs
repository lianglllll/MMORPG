using UnityEngine;
using UnityEngine.UI;
using GameClient.Entities;

public class EliteScript : MonoBehaviour
{
    private Image m_HPImage;
    private Image m_MPImage;
    private Text m_PlayerName;
    private BuffGroupScript m_buffGroup;
    private Actor actor;                //UI显示要的目标

    private void Awake()
    {
        m_PlayerName = transform.Find("Name").GetComponent<Text>();
        m_HPImage = transform.Find("HPBar/Image").GetComponent<Image>();
        m_MPImage = transform.Find("MPBar/Image").GetComponent<Image>();
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
        RefreshUI();
    }

    /// <summary>
    /// 刷新UI
    /// </summary>
    public void RefreshUI()
    {
        if (actor == null) return;
        float hp = actor.Hp / actor.MaxHp;
        float mp = actor.Mp / actor.MaxMp;
        m_HPImage.fillAmount = hp;
        m_MPImage.fillAmount = mp;
        m_PlayerName.text = actor.ActorName;
    }
}
