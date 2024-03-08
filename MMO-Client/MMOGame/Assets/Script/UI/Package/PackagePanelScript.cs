using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 背包面板
/// </summary>
///

public enum PackageMode
{
    normal,
    delete,
    sort,
}

public class PackagePanelScript : BasePanel
{
    private Transform UIMenu;
    private Transform UIMenuWeapon;
    private Transform UIMenuFood;
    private Transform UITabName;
    private Transform UICloseBtn;
    private Transform UICenter;
    private Transform UIScrollView;
    private Transform UIDetailPanel;
    private Transform UILeftBtn;
    private Transform UIRightBtn;
    private Transform UIDeletePanel;
    private Transform UIDeleteBackBtn;
    private Transform UIDeleteInfoText;
    private Transform UIDeleteConfirmBtn;
    private Transform UIBottomMenus;
    private Transform UIDeleteBtn;
    private Transform UIDetailBtn;

    public GameObject PackageUIItemPrefab;

    private string _chooseUid;
    public string chooseUID
    {
        get
        {
            return _chooseUid;
        }
        set
        {
            _chooseUid = value;
            RefreshDetail();
        }
    }

    public List<string> deleteChooseUid;

    //当前界面的状态
    public PackageMode curMode = PackageMode.normal;


    protected override void Awake()
    {
        base.Awake();
        InitUI();
    }

    private void InitUI()
    {
        InitUIName();
        InitClick();
    }

    protected override void Start()
    {
        RefreshUI();
    }

    /// <summary>
    /// 刷新ui
    /// </summary>
    private void RefreshUI()
    {
        RefreshScroll();
    }

    /// <summary>
    /// 刷新滚动容器
    /// </summary>
    private void RefreshScroll()
    {
        //清理滚动容器中原本的物品
        RectTransform scrollContent =  UIScrollView.GetComponent<ScrollRect>().content;
        for(int i = 0; i < scrollContent.childCount; i++)
        {
            Destroy(scrollContent.GetChild(i).gameObject);
        }

        //获取所有的背包数据
        foreach(var localData in GameController._instance.GetSortPackageLocalData())
        {
            //生成
            Transform packageUIItem = Instantiate(PackageUIItemPrefab.transform, scrollContent) as Transform;
            PackageCell packageCell = packageUIItem.GetComponent<PackageCell>();
            packageCell.Refresh(localData, this);
        }
    }

    /// <summary>
    /// 刷新详情页
    /// </summary>
    private void RefreshDetail()
    {
        PackageLocalItem item = GameController._instance.GetPackageLocalItemById(chooseUID);
        UIDetailPanel.GetComponent<PackageDetailPanelScript>().Refresh(item, this);
    }

    /// <summary>
    /// 获取组件
    /// </summary>
    private void InitUIName()
    {
        UIMenu = transform.Find("CenterTop/Menu");
        UIMenuWeapon = transform.Find("CenterTop/Menu/Weapon");
        UIMenuFood = transform.Find("CenterTop/Menu/Food"); 
        UITabName = transform.Find("LeftTop/TabName");
        UICloseBtn = transform.Find("RightTop/CloseBtn");
        UICenter = transform.Find("Center");
        UIScrollView = transform.Find("Center/Scroll View");
        UIDetailPanel = transform.Find("Center/DetailPanel");
        UILeftBtn = transform.Find("Left/Button");
        UIRightBtn = transform.Find("Right/Button");

        UIDeletePanel = transform.Find("Bottom/DeletePanel");
        UIDeleteBackBtn = transform.Find("Bottom/DeletePanel/BackBtn");
        UIDeleteInfoText = transform.Find("Bottom/DeletePanel/InfoText");
        UIDeleteConfirmBtn = transform.Find("Bottom/DeletePanel/ConFirmBtn");
        UIBottomMenus = transform.Find("Bottom/BottomMenus"); 
        UIDeleteBtn = transform.Find("Bottom/BottomMenus/DeleteBtn");
        UIDetailBtn = transform.Find("Bottom/BottomMenus/DetailBtn");


        UIDeletePanel.gameObject.SetActive(false);
        UIBottomMenus.gameObject.SetActive(true);
    }

    /// <summary>
    /// 添加待删除物品
    /// </summary>
    /// <param name="uid"></param>
    public void AddChooseDeleteUid(string uid)
    {
        this.deleteChooseUid ??= new List<string>();
        if (!this.deleteChooseUid.Contains(uid))
        {
            this.deleteChooseUid.Add(uid);
        }
        else
        {//加两回等于没加
            this.deleteChooseUid.Remove(uid);
        }
        RefreshDeletePanel();
    }

    /// <summary>
    /// 刷新全部ui的删除选中
    /// </summary>
    private void RefreshDeletePanel()
    {
        RectTransform scrollContent = UIScrollView.GetComponent<ScrollRect>().content;
        foreach(Transform cell in scrollContent)
        {
            PackageCell packageCell = cell.GetComponent<PackageCell>();
            packageCell.RefreshDeleteState();
        }
    }



    /// <summary>
    /// 给按钮添加事件
    /// </summary>
    private void InitClick()
    {
        UIMenuWeapon.GetComponent<Button>().onClick.AddListener(OnClickWeapon);
        UIMenuFood.GetComponent<Button>().onClick.AddListener(OnClickFood);
        UICloseBtn.GetComponent<Button>().onClick.AddListener(OnClickClose);
        UILeftBtn.GetComponent<Button>().onClick.AddListener(OnClickLeft);
        UIRightBtn.GetComponent<Button>().onClick.AddListener(OnClickRight);
        UIDeleteBackBtn.GetComponent<Button>().onClick.AddListener(OnClickDeleteBack);
        UIDeleteConfirmBtn.GetComponent<Button>().onClick.AddListener(OnClickDeleteConfirm);
        UIDeleteBtn.GetComponent<Button>().onClick.AddListener(OnClicDelete);
        UIDetailBtn.GetComponent<Button>().onClick.AddListener(OnClickDetail);
    }

    private void OnClickWeapon()
    {
        print(">>>>>OnClickWeapon");
    }

    private void OnClickFood()
    {
        print(">>>>>OnClickFood");
    }

    private void OnClickClose()
    {
        UIManager.Instance.ClosePanel("PackagePanel");
    }

    private void OnClickLeft()
    {
        print(">>>>>OnClickLeft");
    }

    private void OnClickRight()
    {
        print(">>>>>OnClickRight");
    }

    /// <summary>
    /// 退出删除模式
    /// </summary>
    private void OnClickDeleteBack()
    {
        curMode = PackageMode.normal;
        UIDeletePanel.gameObject.SetActive(false);
        deleteChooseUid = new List<string>();
        RefreshDeletePanel();
    }

    /// <summary>
    /// 确认删除
    /// </summary>
    private void OnClickDeleteConfirm()
    {
        if (this.deleteChooseUid == null) return;
        if (this.deleteChooseUid.Count == 0) return;
        GameController._instance.DeletePackageItems(this.deleteChooseUid);
        //Refresh
        RefreshUI();
    }

    /// <summary>
    /// 进入删除模式
    /// </summary>
    private void OnClicDelete()
    {
        curMode = PackageMode.delete;
        UIDeletePanel.gameObject.SetActive(true);
    }

    private void OnClickDetail()
    {
        print(">>>>>OnClickDetail");
    }
}
