using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEffectManager
{
    /// <summary>
    /// 添加特效到世界
    /// </summary>
    /// <returns>The world effect.</returns>
    /// <param name="path">资源路径.</param>
    /// <param name="pos">世界坐标.</param>
    /// <param name="scale">缩放.</param>
    public static GameEffect AddEffectLoc(string path, Vector3 pos, float scale=1f,float duration = -1f)
    {
        GameEffect ge = CreateEffect(path);
        if (ge == null) return null;
        ge.transform.position = pos;
        ge.Play(scale, duration);
        return ge;
    }

    /// <summary>
    /// 添加特效到目标
    /// </summary>
    /// <param name="path">特效路径</param>
    /// <param name="target">目标对象</param>
    /// <param name="offset">相对于目标的偏移</param>
    /// <param name="scale">缩放</param>
    /// <returns></returns>
    public static GameEffect AddEffectTarget(string path, GameObject target, Vector3 offset=default, float scale=1f, float duration = -1f)
    {
        if (target == null)
        {
            return AddEffectLoc(path, offset, scale);
        }
        GameEffect ge = CreateEffect(path);
        if (ge == null) return null;
        ge.SetTarget(target);
        ge.offset = offset;
        ge.Play(scale, duration);
        return ge;
    }

    /// <summary>
    /// 创建一个特效
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private static GameEffect CreateEffect(string path)
    {
        GameObject temp = Res.LoadAssetSync<GameObject>(path);
        if (temp == null)
        {
            Debug.LogError("cant find file ! : " + path);
        }
        GameObject obj = GameObject.Instantiate(temp);
        GameEffect eff = obj.AddComponent<GameEffect>();
        eff.Init();
        return eff;
    }

}
