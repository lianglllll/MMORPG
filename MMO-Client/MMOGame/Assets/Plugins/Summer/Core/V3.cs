using HS.Protobuf.Common;
using UnityEngine;


//工具类  Nvector3--->Vector3
public class V3
{
    public static Vector3 Of(Vec3 nv)
    {
        return new Vector3(nv.X, nv.Y, nv.Z);
    }
    public static Vector3 Of(Vector3 v)
    {
        return new Vector3(v.x, v.y, v.z);
    }
    public static Vector3 ToVector3(Vec3 nv)
    {
        return new Vector3(nv.X, nv.Y, nv.Z);
    }
    public static NetVector3 ToNetVector3(Vector3 v)
    {
        return new NetVector3() { X = (int)v.x, Y = (int)v.y, Z = (int)v.z };
    }

}
