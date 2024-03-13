using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proto;
using GameClient.Entities;

/// <summary>
/// entity网络同步对象
/// todo 这个脚本职责不单一，抽离重力功能，抽离摄像机的功能，只留下pos+dir+state的同步设置
/// </summary>
public class GameEntity : MonoBehaviour
{
    //标志
    private bool startFlag;
    private bool isStartCoroutine;

    private bool isMine;
    private WaitForSeconds waitForSeconds = new WaitForSeconds(0.1f);//同步时间控制

    //entity信息
    public int entityId => owner.EntityId;
    public float speed => owner.info.Speed;
    public string entityName => owner.info.Name;

    public Vector3 position;
    public Vector3 direction;


    private PlayerStateMachine stateMachine;
    public Actor owner;

    //重力
    private CharacterController characterController;
    public float fallSpeed = 0f;//下落速度
    public float FALLSPEEDMAX = 30f;//最大下落速度


    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        stateMachine = GetComponent<PlayerStateMachine>();
    }

    private void Start()
    {
        isStartCoroutine = false;
    }

    private void Update()
    {
        if (!startFlag) return;

        //模拟重力
        if (!characterController.isGrounded)
        {
            //本次的重力加速度向量，也就是重力增量
            //Physics.gravity   其实就是-9.8f
            fallSpeed += 9.8f * Time.deltaTime;
            if (fallSpeed >= FALLSPEEDMAX)
            {
                fallSpeed = FALLSPEEDMAX;
            }
            //向下移动
            characterController.Move(new Vector3(0, -fallSpeed * Time.deltaTime, 0));
        }
        else
        {
            characterController.Move(new Vector3(0, -0.01f, 0));
            fallSpeed = 0f;
        }


        if (!isMine)
        {
            if (owner.IsDeath) return;
            //进行插值处理，而不是之间瞬移，看上去更加平滑
            //因为我们是0.2秒同步一次信息所以是5帧
            Move(Vector3.Lerp(transform.position, position, Time.deltaTime * 5f));
            
            //四元数，插值处理
            Quaternion targetQuaternion = Quaternion.Euler(direction);
            this.transform.rotation = Quaternion.Lerp(transform.rotation, targetQuaternion, Time.deltaTime * 10f);
        }
        else
        {
            //获取玩家控制的角色的位置和角度，我们自己的角色不受网络控制
            //只做记录用
            this.position = transform.position;
            this.direction = transform.rotation.eulerAngles;//记录的是欧拉角
        }

    }

    /// <summary>
    /// 用于显示角色名
    /// </summary>
    private void OnGUI()
    {
        if (!isView(gameObject))
        {
            return;
        }

        float height = 1.8f;

        Camera playerCamera = Camera.main;
        //计算角色头顶往上一点的世界坐标
        Vector3 targetPos = new Vector3(transform.position.x, transform.position.y + height, transform.position.z);
        //转换为screen坐标
        Vector2 screenPos =  playerCamera.WorldToScreenPoint(targetPos);
        //screen坐标转换为gui坐标
        Vector2 guiPos = new Vector2(screenPos.x, Screen.height - screenPos.y);


        //label样式
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.fontSize = 24; // 设置适当的字体大小，这里的值为20


        //根据姓名的长度做一些位置调整
        //获取通过gui生成名字的标签的宽高
        //Vector2 nameSize = GUI.skin.label.CalcSize(new GUIContent(entityName));
        Vector2 nameSize = labelStyle.CalcSize(new GUIContent(entityName));


        //设置gui的颜色
        GUI.color = Color.green;
        //绘制gui
        //nameLabel你需要将其x轴向做移动标准长度的一半，否则标签会从guipos这个点直接开始
        Rect nameLabel = new Rect(guiPos.x-(nameSize.x/2),guiPos.y-nameSize.y,nameSize.x,nameSize.y);
        GUI.Label(nameLabel,entityName,labelStyle);

    }

    /// <summary>
    /// 启动GameEntity
    /// </summary>
    /// <param name="actor"></param>
    public void _Start(Actor actor,bool ismine, Vector3 pos,Vector3 dir)
    {
        if (startFlag) return;
        if (actor == null) return;

        owner = actor;
        isMine = ismine;
        startFlag = true;

        this.position = pos;
        this.direction = dir;

        if (ismine)
        {
            //开启同步信息功能的协程
            //开启协程，每秒发送10次向服务器上传hero的属性
            StartCoroutine(SyncRequest());
        }

    }

    /// <summary>
    /// 关闭GameEntity
    /// </summary>
    public void _Stop()
    {
        if (!startFlag) return;
        startFlag = false;
        if (isStartCoroutine)
        {
            StopCoroutine(SyncRequest());
        }
    }

    /// <summary>
    /// 根据向量立刻移动
    /// </summary>
    /// <param name="target"></param>
    public void Move(Vector3 target)
    {
        characterController.Move(target - transform.position);
    }

    /// <summary>
    /// 判断一个对象是否在摄像机的显示范围内
    /// </summary>
    /// <param name="targetObj"></param>
    /// <returns></returns>
    public bool isView(GameObject targetObj)
    {
        Vector3 worldPos = targetObj.transform.position;
        Transform camTransform = Camera.main.transform;
        //距离50米
        if (Vector3.Distance(camTransform.position, worldPos) > 50f)
        {
            return false;
        }
        Vector2 viewPos = Camera.main.WorldToViewportPoint(worldPos);
        Vector3 dir = (worldPos - camTransform.position).normalized;
        //判断物体是否在相机前
        float dot = Vector3.Dot(camTransform.forward, dir);
        if(dot>0 && viewPos.x>=0 && viewPos.x<=1 && viewPos.y >= 0 && viewPos.y <= 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 发送同步信息协程
    /// </summary>
    /// <returns></returns>
    IEnumerator SyncRequest()
    {
        //优化,防止不断在堆中创建新对象
        SpaceEntitySyncRequest req = new SpaceEntitySyncRequest()
        {
            EntitySync = new NEntitySync()
            {
                Entity = new NetEntity()
                {
                    Position = new Vec3(),
                    Direction = new Vec3()
                }
            }
        };

        while (true)
        {
            //只有当主角移动的时候才会发生同步信息
            if (transform.hasChanged)
            {
                SetValueTo(this.position * 1000, req.EntitySync.Entity.Position);
                SetValueTo(this.direction * 1000, req.EntitySync.Entity.Direction);
                req.EntitySync.Entity.Id = entityId;
                //如果角色现在的动画处于不是常规的motion动作时，同一传none
                req.EntitySync.State = stateMachine.currentEntityState;
                NetClient.Send(req);

                //重置
                transform.hasChanged = false;
                req.EntitySync.State = EntityState.NoneState;
            }
            yield return waitForSeconds;
        }

    }

    /// <summary>
    /// 设置同步包中的方向和位置vec数据
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    private void SetValueTo(Vector3 a, Vec3 b)
    {
        b.X = (int)a.x;
        b.Y = (int)a.y;
        b.Z = (int)a.z;
    }

    /// <summary>
    /// 响应：同步位置+方向+速度信息
    /// </summary>
    /// <param name="nEntity"></param>
    /// <param name="instantMove">是否直接设置到transform.position</param>
    public void SetData(NetEntity nEntity, bool instantMove = false)
    {
        if (startFlag)
        {
            this.position = ToVector3(nEntity.Position);
            this.direction = ToVector3(nEntity.Direction);

            if (instantMove)
            {
                transform.rotation = Quaternion.Euler(direction);
                transform.position = position;
            }

        }

    }

    /// <summary>
    /// 将Nvector3转换为unity的vector3，数值缩小1000倍
    /// </summary>
    /// <param name="v3"></param>
    /// <returns></returns>
    private Vector3 ToVector3(Vec3 v3)
    {
        return new Vector3 { x = v3.X * 0.001f, y = v3.Y * 0.001f, z = v3.Z * 0.001f };
    }

}
