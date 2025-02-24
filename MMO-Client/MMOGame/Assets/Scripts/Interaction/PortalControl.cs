using GameClient.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalControl : InteractionBehaviour
{
    public int tagetSpaceId;
    public int pointId;

    protected override void Interaction()
    {
        throw new NotImplementedException();
    }

    private void OnTriggerEnter(Collider other)
    {
        //非本机玩家不触发
        if (!other.CompareTag("CtlPlayer")) return;

        //设置数据
        var spaceDefine = LocalDataManager.Instance.GetSpaceDefineById(tagetSpaceId);
        string DeliverText;     //提示语句
        Action onBtnAction = null ;

        if (spaceDefine != null)
        {
            DeliverText = "传送到目标：" + spaceDefine.Name+"是否传送？";
            //事件回调
            onBtnAction = () =>
            {
                int spaceId = tagetSpaceId;//闭包
                if (spaceId < 0)
                {
                    UIManager.Instance.ShowTopMessage("传送失败,无法搜寻坐标点");
                    return;
                }
                //给服务器发送请求
                CombatService.Instance.SpaceDeliver(spaceId, pointId);
            };


        }
        else
        {
            DeliverText = "由于空间乱流，目标点暂时无法传送";
        }


        //调用uimananger，显示传送面板
        UIManager.Instance.MessagePanel.ShowSelectionPanel("传送", DeliverText, onBtnAction);
    }

    private void OnTriggerExit(Collider other)
    {
        UIManager.Instance.MessagePanel.CloseSelectionPanel();
    }


}
