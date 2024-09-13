using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteraction
{
    /// <summary>
    /// 交互行为
    /// </summary>
    void InteractionAction();

    /// <summary>
    /// 是否可以交互
    /// </summary>
    /// <returns></returns>
    bool CanInteraction();

}
