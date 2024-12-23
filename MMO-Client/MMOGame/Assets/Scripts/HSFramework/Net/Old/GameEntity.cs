
/// <summary>
/// entity网络同步对象
/// </summary>
/*public class GameEntity : MonoBehaviour
{
    //标志
    private bool startFlag;
    private bool isStartCoroutine;

    private bool isMine;
    private WaitForSeconds waitForSeconds = new WaitForSeconds(0.1f);//同步时间控制

    //entity信息
    public int entityId => owner.EntityId;

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
            fallSpeed += 9.8f * Time.deltaTime;
            if (fallSpeed >= FALLSPEEDMAX)
            {
                fallSpeed = FALLSPEEDMAX;
            }
        }
        else
        {
            fallSpeed = 1f;
        }
        //向下移动
        characterController.Move(new Vector3(0, -fallSpeed * Time.deltaTime, 0));


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
    /// 启动GameEntity
    /// </summary>
    /// <param name="actor"></param>
    public void _Start(Actor actor,bool ismine, Vector3 pos,Vector3 dir)
    {
        if (startFlag) return;

        if (actor == null)
        {
            Destroy(gameObject);
            return;
        }

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
        if (characterController == null || !characterController.enabled) return;
        target.y = transform.position.y;
        characterController.Move(target - transform.position);
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
                SetValueTo(transform.position * 1000, req.EntitySync.Entity.Position);
                SetValueTo(transform.rotation.eulerAngles * 1000, req.EntitySync.Entity.Direction);
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
            var pos = ToVector3(nEntity.Position);
            this.position.x = pos.x;
            //y值不变
            this.position.z = pos.z;
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
*/
