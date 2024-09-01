using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffUIScript : MonoBehaviour
{
    private Image icon;
    private Image coldDownLayer;
    private TextMeshProUGUI levelText;

    public Buff _buff;

    private void Awake()
    {
        icon = transform.Find("Icon").GetComponent<Image>();
        coldDownLayer = transform.Find("ColdDownLayer").GetComponent<Image>();
        levelText = transform.Find("LevelText").GetComponent<TextMeshProUGUI>();
    }

    public void Init(Buff buff)
    {
        //设置ui icon
        this._buff = buff;
    }

    public void UpdateUI(float deltaTime)
    {
        if (_buff == null) return;
        if (icon.sprite == null)
        {
            icon.sprite = Res.LoadAssetSync<Sprite>(_buff.IconPath,FileType.Png);
        }
        coldDownLayer.fillAmount = 1 - (_buff.ResidualDuration / _buff.MaxDuration);
        levelText.text = _buff.CurrentLevel.ToString();
        levelText.gameObject.SetActive(_buff.CurrentLevel > 1);

    }


}
