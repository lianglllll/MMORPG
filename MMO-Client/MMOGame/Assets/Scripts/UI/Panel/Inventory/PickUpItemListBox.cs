using GameClient.Entities;
using GameServer.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItemListBox : MonoBehaviour
{
    private GameObject pickUpItemCellPrefab;
    private Transform Content;              //放置cell的父物体

    private void Awake()
    {
        pickUpItemCellPrefab = Res.LoadAssetSync<GameObject>("UI/Prefabs/Inventory/PickUpItemCell.prefab");
        Content = transform.Find("Scroll View/Viewport/Content").transform;
    }

    /// <summary>
    /// 重新设置
    /// </summary>
    /// <param name="itemEntitys"></param>
    public void Reset(List<ItemEntity> itemEntitys)
    {
        //清理content下的子物体
        int childCount = Content.childCount;
        for (int i = 0; i < childCount; ++i)
        {
            var childObj = Content.GetChild(i);
            Destroy(childObj.gameObject);
        }

        //添加新的子物体
        foreach (var ie in itemEntitys)
        {
            var obj = Instantiate(pickUpItemCellPrefab, Content);
            var pickUpItemCell = obj.GetComponent<PickUpItemCell>();
            pickUpItemCell.Init(ie);

        }
    }

}
