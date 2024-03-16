using Serilog;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 背包中物品交互用的
/// </summary>
public class ItemMenu : MonoBehaviour
{

    public static ItemMenu Instance;

    public delegate void MenuOptionCallback(string value);

    public GameObject menuOptionPrefab;

    public MenuOptionCallback optionCallback;
    

    private void Awake()
    {
        Instance = this;
        transform.gameObject.SetActive(false);
    }


    public static void Show(Vector3 pos, string[] values, MenuOptionCallback callback)
    {
        Instance.optionCallback = callback;
        Instance.transform.position = pos;
        // 删除全部Node
        foreach (var node in Instance.transform.GetComponentsInChildren<ItemMenuOption>())
        {
            Destroy(node.gameObject);
        }

        // 创建菜单项
        foreach (string value in values)
        {
            Instance.CreateMenuItem(value);
        }
        // 显示菜单
        Instance.transform.gameObject.SetActive(true);
        //Instance.transform.SetAsLastSibling();
    }

    public static void Hide()
    {
        Instance.transform.gameObject.SetActive(false);
    }

    public void CreateMenuItem(string label)
    {
        // 创建菜单项
        GameObject menuItemObject = Instantiate(menuOptionPrefab, transform);
        var menuItem = menuItemObject.GetComponent<ItemMenuOption>();
        menuItem.SetLabel(label);
    }
}
