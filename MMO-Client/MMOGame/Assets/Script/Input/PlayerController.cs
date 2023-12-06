using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 玩家控制器需要挂载到玩家对象上
/// </summary>
public class PlayerController : MonoBehaviour
{
    private PlayerInput input;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        //启用GamePlay动作表
        input.EnableGameplayInputs();
    }


}
