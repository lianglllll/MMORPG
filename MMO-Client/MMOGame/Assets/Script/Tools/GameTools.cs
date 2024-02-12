using GameClient.Entities;
using Summer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 工具类
/// </summary>
public class GameTools
{
    /// <summary>
    /// 计算地面坐标点
    /// </summary>
    /// <param name="position"></param>
    /// <param name="up"></param>
    /// <param name="ignoreLayer"></param>
    /// <returns></returns>
    public static Vector3 CaculateGroundPosition(Vector3 position,float upOffset,int ignoreLayer)
    {
        Vector3 tmp = position + new Vector3(0, 1000f, 0);//物体位置上面1000个单位
        RaycastHit hit;
        int layerMask = ~(1 << ignoreLayer);
        //向下发送射线
        if(Physics.Raycast(tmp, Vector3.down, out hit,Mathf.Infinity, layerMask))//Mathf.Infinity表示无穷远
        {
            tmp = hit.point;
            tmp.y += upOffset;
            return tmp;
        }
        else
        {
            tmp = position;
            tmp.y += upOffset;
            return tmp;
        }
         
    }

    /// <summary>
    /// 根据entityid获取actor
    /// </summary>
    /// <param name="entityId"></param>
    /// <returns></returns>
    public static Actor GetUnit(int entityId)
    {
        return EntityManager.Instance.GetEntity<Actor>(entityId);
    }
    

}
