using GameClient.Entities;
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
        if (!other.CompareTag("CtlPlayer")) return;
        //调用uimananger，显示传送面板
        UIManager.Instance.MessagePanel.ShowDeliverBox(tagetId);
    }

    private void OnTriggerExit(Collider other)
    {
        UIManager.Instance.MessagePanel.CloseDeliverBox();
    }

}
