using GameClient.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour,IInteraction 
{

    public int tagetId;

    public bool CanInteraction()
    {
        throw new System.NotImplementedException();
    }

    public void InteractionAction()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        //非本机玩家不触发
        if (!other.CompareTag("CtlPlayer")) return;

        //设置数据
        var spaceDefine = DataManager.Instance.GetSpaceDefineById(tagetId);
        string DeliverText;     //提示语句
        bool btnActive = false; //是否启用按钮触发回调

        if (spaceDefine != null)
        {
            DeliverText = "目标：" + spaceDefine.Name;
            btnActive = true;
        }
        else
        {
            DeliverText = "由于空间乱流，目标点暂时无法传送";
            btnActive = false;
        }

        //事件回调
        Action onBtnAction = () =>
        {
            int spaceId = tagetId;//闭包
            if (spaceId < 0)
            {
                UIManager.Instance.ShowMessage("传送失败,无法搜寻坐标点");
                return;
            }
            //给服务器发送请求
            GameApp.SpaceDeliver(spaceId);
        };

        //调用uimananger，显示传送面板
        UIManager.Instance.MessagePanel.ShowConfirmBox(DeliverText,"传送",btnActive, onBtnAction);
    }

    private void OnTriggerExit(Collider other)
    {
        UIManager.Instance.MessagePanel.CloseConfirmBox();
    }


}
