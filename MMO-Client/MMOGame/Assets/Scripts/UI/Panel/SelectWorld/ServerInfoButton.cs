using GameClient;
using UnityEngine;
using UnityEngine.EventSystems;


public class ServerInfoButton : MonoBehaviour,IPointerDownHandler
{

    public ServerInfo serverInfo;

    public void OnPointerDown(PointerEventData eventData)
    {
        GameApp.ServerInfo = this.serverInfo;

        PlayerPrefs.SetString("myServerInfo", JsonUtility.ToJson(GameApp.ServerInfo));
        PlayerPrefs.Save();
    }


}
