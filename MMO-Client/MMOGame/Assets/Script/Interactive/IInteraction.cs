using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteraction
{
    /// <summary>
    /// ������Ϊ
    /// </summary>
    void InteractionAction();

    /// <summary>
    /// �Ƿ���Խ���
    /// </summary>
    /// <returns></returns>
    bool CanInteraction();

}
