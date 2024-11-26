using BaseSystem.PoolModule;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 自定义scoll view类 用于节约性能 通过缓存池创建复用对象
/// </summary>
/// <typeparam name="T">代表的 数据来源类</typeparam>
/// <typeparam name="K">代表的 格子类</typeparam>
public class RecyclingListView<T,K> where K:IRecyclingListViewItem<T>
{
    private ScrollRect scrollRect;
    private RectTransform content;
    private float viewPortH;
    
    private List<T> items;
    private Dictionary<int, GameObject> nowShowItemUIs = new Dictionary<int, GameObject>();

    //记录上一次显示的索引范围
    private int oldMinIndex = -1;
    private int oldMaxIndex = -1;
    //格子的间隔宽高
    private int itemW;
    private int itemH;
    //格子的列数
    private int col;
    //预设体资源的路径
    private string itemPath;

    public void Init(ScrollRect scrollRect, int w, int h, int col, string itemPath, List<T> items)
    {
        this.scrollRect = scrollRect;
        //初始化Content父对象 以及 我们可视范围的高
        this.viewPortH = scrollRect.GetComponent<RectTransform>().rect.height;
        this.content = scrollRect.content;

        //初始化格子间隔大小 以及 一行几列
        this.itemW = w;
        this.itemH = h + 10;
        this.col = col;

        // 初始化格子资源路径
        this.itemPath = itemPath;

        //应该要初始化履带的长度content的高
        this.items = items;
        content.sizeDelta = new Vector2(0, Mathf.CeilToInt(items.Count / col) * itemH);

        CheckShowOrHide();

        //事件监听
        scrollRect.onValueChanged.AddListener(OnChange);
    }

    private void OnChange(Vector2 arg0)
    {
        CheckShowOrHide();
    }

    /// <summary>
    /// 更新格子显示的方法
    /// </summary>
    public void CheckShowOrHide()
    {
        //检测哪些格子应该显示出来
        int minIndex = (int)(content.anchoredPosition.y / itemH) * col;
        int maxIndex = (int)((content.anchoredPosition.y + viewPortH) / itemH) * col + col - 1;

        //最小值判断
        if (minIndex < 0)
            minIndex = 0;

        //超出道具最大数量
        if (maxIndex >= items.Count)
            maxIndex = items.Count - 1;

        if( minIndex != oldMinIndex || maxIndex != oldMaxIndex)
        {
            //在记录当前索引之前 要做一些事儿
            //根据上一次索引和这一次新算出来的索引 用来判断 哪些该移除
            //删除上一节溢出
            for (int i = oldMinIndex; i < minIndex; ++i)
            {
                if (nowShowItemUIs.ContainsKey(i))
                {
                    if (nowShowItemUIs[i] != null)
                        UnityObjectPoolFactory.Instance.RecycleItem(itemPath, nowShowItemUIs[i]);
                    nowShowItemUIs.Remove(i);
                }
            }
            //删除下一节溢出
            for (int i = maxIndex + 1; i <= oldMaxIndex; ++i)
            {
                if (nowShowItemUIs.ContainsKey(i))
                {
                    if (nowShowItemUIs[i] != null)
                        UnityObjectPoolFactory.Instance.RecycleItem(itemPath, nowShowItemUIs[i]);
                    nowShowItemUIs.Remove(i);
                }
            }
        }

        oldMinIndex = minIndex;
        oldMaxIndex = maxIndex;

        //创建指定索引范围内的格子
        for (int i = minIndex; i <= maxIndex; ++i)
        {
            if (nowShowItemUIs.ContainsKey(i))
                continue;
            else
            {
                //根据这个关键索引 用来设置位置 初始化道具信息
                int index = i;
                nowShowItemUIs.Add(index, null);

                var obj = UnityObjectPoolFactory.Instance.GetItem<GameObject>(itemPath);

                //当格子创建出来后我们要做什么
                //设置它的父对象
                obj.transform.SetParent(content);
                //重置相对缩放大小
                obj.transform.localScale = Vector3.one;
                //重置位置
                obj.transform.localPosition = new Vector3((index % col) * itemW, -index / col * itemH, 0);
                //更新格子信息
                obj.GetComponent<K>().InitInfo(items[index]);

                //判断有没有这个坑
                if (nowShowItemUIs.ContainsKey(index))
                    nowShowItemUIs[index] = obj;
                else
                    UnityObjectPoolFactory.Instance.RecycleItem(itemPath, obj);
            }
        }

    }
}
