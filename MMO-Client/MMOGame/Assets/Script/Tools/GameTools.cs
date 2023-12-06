using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTools 
{

    //推测地面坐标点
    public static Vector3 CaculateGroundPosition(Vector3 position,float up = 1000,int ignoreLayer = 6)
    {
        Vector3 tmp = position + new Vector3(0, 2000f, 0);//物体位置上面1000个单位
        RaycastHit hit;
        int layerMask = ~(1 << ignoreLayer);
        if(Physics.Raycast(tmp, Vector3.down, out hit,Mathf.Infinity, layerMask))//Mathf.Infinity表示无穷远
        {
            return hit.point;
        }
        else
        {
            return position;
        }
         
    }
}
