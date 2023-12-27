using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PackageDetailPanelScript : MonoBehaviour
{
    private Transform UIStars;
    private Transform UIDescription;
    private Transform UIIcon;
    private Transform UITitle;
    private Transform UILevelText;
    private Transform UISkillDescription;

    private PackageLocalItem localItem;
    private PackageTableItem tableItem;
    private PackagePanelScript uiParent;






    private void Awake()
    {
        InitUIName();
    }



    /// <summary>
    /// 获取组件
    /// </summary>
    private void InitUIName()
    {
        UIStars = transform.Find("Center/Stars");
        UIDescription = transform.Find("Center/Description");
        UIIcon = transform.Find("Center/Icon");
        UITitle = transform.Find("Top/Title");
        UILevelText = transform.Find("Bottom/LevelPanel/LevelText");
        UISkillDescription = transform.Find("Bottom/Description");
    }

    /// <summary>
    /// 刷新ui
    /// </summary>
    /// <param name="localItem"></param>
    /// <param name="packagePanelScript"></param>
    public void Refresh(PackageLocalItem localItem,PackagePanelScript packagePanelScript)
    {
        this.uiParent = packagePanelScript;
        this.localItem = localItem;
        this.tableItem = GameController._instance.GetPackageItemById(localItem.id);
        //level
        UILevelText.GetComponent<Text>().text = string.Format("Lv.{0}/40", this.localItem.level.ToString());
        // less desc
        UIDescription.GetComponent<Text>().text = this.tableItem.descriptions;
        //more desc
        UISkillDescription.GetComponent<Text>().text = this.tableItem.detailDescriptions;
        //name
        UITitle.GetComponent<Text>().text = this.tableItem.name;
        //image
        Texture2D t = (Texture2D)Resources.Load(this.tableItem.imagePath);
        Sprite temp = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0, 0));
        UIIcon.GetComponent<Image>().sprite = temp;
        //stars
        RefreshStars();
    }


    /// <summary>
    /// 刷新星星
    /// </summary>
    public void RefreshStars()
    {
        for (int i = 0; i < UIStars.childCount; i++)
        {
            Transform star = UIStars.GetChild(i);
            if (this.tableItem.star > i)
            {
                star.gameObject.SetActive(true);
            }
            else
            {
                star.gameObject.SetActive(false);
            }
        }
    }

}
