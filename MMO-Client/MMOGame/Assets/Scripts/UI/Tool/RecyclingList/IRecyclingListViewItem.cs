using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//锚点可以丢到左上角
/// <summary>
/// 该接口 作为 格子对象 必须继承的类 它用于实现初始化格子的方法
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IRecyclingListViewItem<T>
{
    void InitInfo(T info);
}
