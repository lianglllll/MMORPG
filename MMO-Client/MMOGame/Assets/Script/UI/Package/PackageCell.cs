using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PackageCell : MonoBehaviour,IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler
{
    private Transform UIIcon;
    private Transform UIHead;
    private Transform UINew;
    private Transform UISelect;
    private Transform UILevel;
    private Transform UIStars;
    private Transform UIDeleteSelect;

    private PackageLocalItem localItem;
    private PackageTableItem tableItem;
    private PackagePanelScript uiParent;

    private Transform UISelectAni;
    private Transform UIMouseOverAni;


    private void Awake()
    {
        InitUIName();
    }

    /// <summary>
    /// 获取组件
    /// </summary>
    private void InitUIName()
    {
        UIIcon = transform.Find("Top/Icon");
        UIHead = transform.Find("Top/Head");
        UINew = transform.Find("Top/New");
        UISelect = transform.Find("Select");
        UILevel = transform.Find("Botton/LevelText");
        UIStars = transform.Find("Botton/Stars");
        UIDeleteSelect = transform.Find("DeletSelect");
        UISelectAni = transform.Find("SelectAni");
        UIMouseOverAni = transform.Find("MouseOverAni");

        UIDeleteSelect.gameObject.SetActive(false);
        UISelectAni.gameObject.SetActive(false);
        UIMouseOverAni.gameObject.SetActive(false);
    }

    /// <summary>
    /// 刷新当前的ui
    /// </summary>
    /// <param name="localData"></param>
    /// <param name="uiParent"></param>
    public void Refresh(PackageLocalItem localData,PackagePanelScript uiParent)
    {
        //数据获取
        this.uiParent = uiParent;
        this.localItem = localData;
        this.tableItem = GameController._instance.GetPackageItemById(localData.id);
        //level
        UILevel.GetComponent<Text>().text = "Lv." + this.localItem.level.ToString();
        //is new
        UINew.gameObject.SetActive(localItem.isNew);
        //物品图片
        Texture2D t = (Texture2D)Resources.Load(this.tableItem.imagePath);
        Sprite temp = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0, 0));
        UIIcon.GetComponent<Image>().sprite = temp;
        //refresh star
        RefreshStars();
    }

    /// <summary>
    /// 刷新星星
    /// </summary>
    public void RefreshStars()
    {
        for(int i = 0; i < UIStars.childCount; i++)
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



    /// <summary>
    /// 鼠标退出事件
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        UIMouseOverAni.gameObject.SetActive(true);
        UIMouseOverAni.GetComponent<Animator>().SetTrigger("Out");
    }

    /// <summary>
    /// 鼠标进入事件
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        UIMouseOverAni.gameObject.SetActive(true);
        UIMouseOverAni.GetComponent<Animator>().SetTrigger("In");
    }

    /// <summary>
    /// 鼠标点击事件
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if(uiParent.curMode == PackageMode.delete)
        {
            this.uiParent.AddChooseDeleteUid(this.localItem.uid);
        }
        
        if (this.uiParent.chooseUID == this.localItem.uid) return;
        uiParent.chooseUID = this.localItem.uid;
        UISelectAni.gameObject.SetActive(true);
        UISelectAni.GetComponent<Animator>().SetTrigger("In");
        


    }

    /// <summary>
    /// 刷新删除状态
    /// </summary>
    public void RefreshDeleteState()
    {
        if (this.uiParent.deleteChooseUid.Contains(localItem.uid))
        {
            this.UIDeleteSelect.gameObject.SetActive(true);
        }
        else
        {
            this.UIDeleteSelect.gameObject.SetActive(false);
        }
    }
}
