using UnityEngine;

public class DrawRangeGizmo : MonoBehaviour
{
    [Header("Gizmo Settings")]
    [SerializeField] private float radius = 35f;         // 圆形半径
    [SerializeField] private Color gizmoColor = Color.green; // 显示颜色
    [SerializeField] private bool alwaysVisible = true;  // 常驻显示开关

    void OnDrawGizmos()
    {
        if (!alwaysVisible) return;

        Gizmos.color = gizmoColor;
        Vector3 center = transform.position;
        Gizmos.DrawWireSphere(center, radius);
    }
}
