using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public struct ItemIOInfoSt
{
    public float coldDown;
    public string msg;
}

public class ItemIOInfoBox : MonoBehaviour
{
    private float showTime = 2f;
    private List<ItemIOInfoItem> ItemIOInfoItemList = new();
    private Queue<ItemIOInfoSt> msgQueue = new();

    private void Awake()
    {
        ItemIOInfoItemList.Add(transform.Find("Grid/ItemIOInfoItem1").GetComponent<ItemIOInfoItem>());
        ItemIOInfoItemList.Add(transform.Find("Grid/ItemIOInfoItem2").GetComponent<ItemIOInfoItem>());
        ItemIOInfoItemList.Add(transform.Find("Grid/ItemIOInfoItem3").GetComponent<ItemIOInfoItem>());

    }

    private void Start()
    {
        foreach (var item in ItemIOInfoItemList)
        {
            item.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if(msgQueue.Count > 0)
        {
            int count = msgQueue.Count;
            bool isChange = false;
            while(count > 0)
            {
                var st = msgQueue.Dequeue();
                st.coldDown -= Time.deltaTime;
                if(st.coldDown >= 0)
                {
                    msgQueue.Enqueue(st);
                }
                else
                {
                    isChange = true;
                }
                --count;
            }
            if (isChange)
            {
                ShowAll();
            }
        }
    }

    private void ShowAll()
    {
        while(msgQueue.Count > 3)
        {
            msgQueue.Dequeue();
        }

        int size = msgQueue.Count;

        foreach (var item in ItemIOInfoItemList)
        {
            if(size <= 0)
            {
                item.gameObject.SetActive(false);
                continue;
            }
            item.gameObject.SetActive(true);

            var st = msgQueue.Dequeue();
            item.ShowMsg(st.msg);

            msgQueue.Enqueue(st);
            --size;
        }
    }

    public void ShowMsg(string msg)
    {
        ItemIOInfoSt st = new ItemIOInfoSt();
        st.coldDown = showTime;
        st.msg = msg;
        msgQueue.Enqueue(st);

        ShowAll();
    }
}
