using GameClient;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SelectServer : MonoBehaviour
{
    public string webUrl;
    public GameObject groupObj;
    public TextMeshProUGUI currentServerName;
    private GameObject serverNode;      

    [Serializable]
    public class RootObject
    {
        public ServerInfo[] ServerList;
    }

    void Start()
    {
        //拉取serversJson文件
        StartCoroutine(GetRequest(webUrl));

        if (groupObj != null && groupObj.transform.childCount > 0)
        {
            // 获取第一个子对象
            serverNode = groupObj.transform.GetChild(0).gameObject;
            serverNode.SetActive(false);
        }

        //加载上次的服务器信息
        string myServerInfo = PlayerPrefs.GetString("myServerInfo");
        if (!string.IsNullOrEmpty(myServerInfo))
        {
            GameApp.ServerInfo = JsonUtility.FromJson<ServerInfo>(myServerInfo);
        }

    }

    private void FixedUpdate()
    {
        if (GameApp.ServerInfo != null)
        {
            currentServerName.text = GameApp.ServerInfo.name;
        }
        
    }


    public void StartGame()
    {
        if (GameApp.ServerInfo == null)
        {
            UIManager.Instance.ShowTopMessage("未选择服务器");
            return;
        }

        //还需要判断这个服务器是否可用才能开始游戏。
        Res.LoadSceneAsync("Game");
    }


    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // 发送请求并等待返回
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error: {webRequest.error}");
            }
            else
            {
                // 成功获取数据
                string json = webRequest.downloadHandler.text;

                // 可以在这里将json字符串解析为对象，或者进行其他处理
                // 例如，解析为自定义的数据结构
                // MyDataObject data = JsonUtility.FromJson<MyDataObject>(json);

                // 解析JSON
                RootObject rootObject = JsonConvert.DeserializeObject<RootObject>(json);

                if (rootObject != null && rootObject.ServerList != null)
                {
                    foreach (ServerInfo server in rootObject.ServerList)
                    {
                        // 创建tempNode实例
                        GameObject inst = Instantiate(serverNode, groupObj.transform);

                        // 设置tempNode实例的属性
                        inst.name = server.name;
                        inst.transform.Find("Icon").GetComponent<Image>();
                        inst.transform.GetComponentInChildren<TextMeshProUGUI>().text = server.name;
                        inst.SetActive(true);
                        var btn = inst.GetComponent<ServerInfoButton>();
                        btn.serverInfo = server;
                    }
                }


            }
        }
    }



}