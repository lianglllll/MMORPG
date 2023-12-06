using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]//添加所需的组件
public class PlayerController2 : MonoBehaviour
{
    //将资源文件直接挂过来就行
    [SerializeField] PlayerInput2 input;

    //刚体
    new Rigidbody2D rigidbody;
    //速度
    [SerializeField] float moveSpeed = 10f;


    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        rigidbody.gravityScale = 0f;
        input.EnableGamePlayInput();//激活player表  
    }



    private void OnEnable()//注册事件
    {
        input.onMove += Move;
        input.onStopMove += StopMove;
    }
    private void OnDisable()
    {
        input.onMove -= Move;
        input.onStopMove -= StopMove;
    }



    void Move(Vector2 moveInput)
    {
        Vector2 moveAmount = moveInput * moveSpeed;
        rigidbody.velocity = moveAmount;
    }

    void StopMove()
    {
        rigidbody.velocity = Vector2.zero;
    }

}
