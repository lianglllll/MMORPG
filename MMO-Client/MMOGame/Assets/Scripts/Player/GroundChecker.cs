using UnityEngine;

/// <summary>
/// 检查目标是否在地面之下
/// </summary>
public class GroundChecker : MonoBehaviour
{
    public LayerMask groundLayer;
    private CharacterController controller;
    public Vector3 offset = new Vector3(0, 2, 0);    // 从物体上方2米发出射线
    private RaycastHit hit;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        groundLayer = LayerMask.GetMask("Ground");
    }

    void Update()
    {
        // 射线检测,从头顶offset作为发射点，向下发射无限远的射线，看看是否检测到地面
        if (!Physics.Raycast(transform.position + Vector3.up * 2, Vector3.down, out _, Mathf.Infinity, groundLayer))
        {
            if (Physics.Raycast(transform.position + Vector3.up * 10000f, Vector3.down, out hit, Mathf.Infinity, groundLayer))
            {
                // 检测到地面，将角色移动到地面上
                float newY = hit.point.y + 1;
                Vector3 newPos = new Vector3(transform.position.x, newY, transform.position.z);
                controller.enabled = false; // 关闭 CharacterController 以直接设置位置
                transform.position = newPos;
                controller.enabled = true; // 重新启用 CharacterController
            }
                
        }

    }
}

