
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIManager 
{
    private static UIManager _instance;
    private Transform _uiRoot;                                          //挂载点（空物体):用于容纳所有的panel，便于管理

    private Dictionary<string, PanelDefine> pathDict = null;           //panelname  -> paneldefine  
    private Dictionary<string, GameObject> prefabsDict = null;         //panelname  -> prefab
    private Dictionary<string, BasePanel> panelScriptDict = null;      //panelname  -> panelscript    这是当前打开的面板

    //用于展示消息的面板（如果有需要可以将panel的实例保存起来使用）
    private MessagePanelScript messagePanel;                            

    private UIManager()
    {
        InitDicts();
    }

    public static UIManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new UIManager();
            }
            return _instance;
        }
    }

    public Transform UIRoot
    {
        get
        {
            if (_uiRoot == null)
            {
                _uiRoot = GameObject.Find("Panel")?.transform;
                if(_uiRoot == null)
                {
                    GameObject emptyObject = new GameObject("Panel");
                    // 可选：你可以设置空物体的位置、旋转等属性
                    emptyObject.transform.position = new Vector3(0f, 0f, 0f);
                    emptyObject.transform.rotation = Quaternion.identity;
                    _uiRoot = emptyObject.transform;
                }
            }

            return _uiRoot;
        }
    }

    /// <summary>
    /// 消息面板
    /// </summary>
    public MessagePanelScript MessagePanel
    {
        get
        {
            if(messagePanel == null)
            {
                messagePanel = GameObject.Find("Panel/InfoPanel").transform.GetComponent<MessagePanelScript>();
            }
            return messagePanel;
        }
    }

    /// <summary>
    /// 管理器初始化
    /// </summary>
    private void InitDicts()
    {
        //获取各个面板的数据
        pathDict = DataManager.Instance.panelDict;
        prefabsDict = new Dictionary<string, GameObject>();
        panelScriptDict = new Dictionary<string, BasePanel>();
    }

    /// <summary>
    /// 打开panel
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public BasePanel OpenPanel(string name)
    {
        //1.检查name是否有误
        PanelDefine define = null;
        if(!pathDict.TryGetValue(name,out define))
        {
            Debug.LogError("界面名称有误：" + name);
            return null;
        }

        //2.检查目标界面是否已经打开了
        BasePanel panel = null;
        if(panelScriptDict.TryGetValue(name,out panel)){
            Debug.Log(name + "界面已经打开");
            return null;
        }

        //3.查看是否有prefab缓冲能用
        GameObject panelPrefab = null;
        if(!prefabsDict.TryGetValue(name,out panelPrefab)){
            panelPrefab = Resources.Load<GameObject>(define.Resource) as GameObject;
            prefabsDict.Add(name, panelPrefab);                             //添加到缓冲
        }

        //4.实例化panel，并且挂载到挂载点
        GameObject panelObject = GameObject.Instantiate(panelPrefab, UIRoot, false);
        panel = panelObject.GetComponent<BasePanel>();
        panelScriptDict.Add(name, panel);
        panel.OpenPanel(name);
        return panel;
    }

    /// <summary>
    /// 关闭panel
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool ClosePanel(string name)
    {

        //1.检查界面是否被打开了
        BasePanel panel = null;
        if(!panelScriptDict.TryGetValue(name,out panel))
        {
            Debug.LogError("界面未打开或不存在panel：" + name);
            return false;
        }

        //2.移除缓冲
        panelScriptDict.Remove(name);
        panel.ClosePanel();
        return true;

    }

    /// <summary>
    /// 获取某个打开的penel
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public BasePanel GetPanelByName(string name)
    {
        if (panelScriptDict.TryGetValue(name, out BasePanel panel))
        {
            return panel;
        }
        return null;
    }

    /// <summary>
    /// 获取某个uiprefab
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public GameObject GetPanelPrefab(string name)
    {
        //1.检查name是否有误
        PanelDefine define = null;
        if (!pathDict.TryGetValue(name, out define))
        {
            Debug.LogError("界面名称有误：" + name);
            return null;
        }

        //2.查看是否有prefab缓冲能用
        GameObject panelPrefab = null;
        if (!prefabsDict.TryGetValue(name, out panelPrefab))
        {
            panelPrefab = Resources.Load<GameObject>(define.Resource) as GameObject;
            prefabsDict.Add(name, panelPrefab);                             //添加到缓冲
        }

        return panelPrefab;
    }

    /// <summary>
    /// 通过消息面板显示消息
    /// </summary>
    /// <param name="str"></param>
    public void ShowTopMessage(string str)
    {
        MessagePanel.ShowTopMsg(str);
    }

    /// <summary>
    /// 异步通过消息面板显示消息
    /// </summary>
    /// <param name="str"></param>
    public void AsyncShowTopMessage(string str)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            MessagePanel.ShowTopMsg(str);
        });
    }

    /// <summary>
    /// 清空当前打开的所有面板
    /// </summary>
    public void ClearAllOpenPanel()
    {
        foreach(var p in panelScriptDict.Values)
        {
            p.ClosePanel();
        }
        panelScriptDict.Clear();
    }

}




