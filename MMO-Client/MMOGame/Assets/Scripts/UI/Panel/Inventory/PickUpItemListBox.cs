using GameClient.Entities;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItemListBox : MonoBehaviour
{
    private GameObject m_pickUpItemCellPrefab;
    private Transform m_content;              // 放置cell的父物体

    private void Awake()
    {
        m_pickUpItemCellPrefab    = Res.LoadAssetSync<GameObject>("UI/Prefabs/Inventory/PickUpItemCell.prefab");
        m_content                 = transform.Find("Scroll View/Viewport/Content").transform;
    }
    /// <summary>
    /// 重新设置
    /// </summary>
    /// <param name="itemEntitys"></param>
    public void ResetUI(List<ClientItem> itemEntitys)
    {
        // 清理content下的子物体
        int childCount = m_content.childCount;
        for (int i = 0; i < childCount; ++i)
        {
            var childObj = m_content.GetChild(i);
            Destroy(childObj.gameObject);
        }

        // 添加新的子物体
        foreach (var ie in itemEntitys)
        {
            var obj = Instantiate(m_pickUpItemCellPrefab, m_content);
            var pickUpItemCell = obj.GetComponent<PickUpItemCell>();
            pickUpItemCell.Init(ie);

        }
    }

}
