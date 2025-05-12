using GameClient.Combat.Buffs;
using GameClient.Entities;
using System.Collections.Generic;
using UnityEngine;

public class BuffGroupScript : MonoBehaviour
{
    private Actor owner;
    private GameObject buffPrefab;
    private Dictionary<int, BuffUIScript> buffUIdict = new();

    private void Start()
    {
        Kaiyun.Event.RegisterOut("SpecialActorAddBuff", this, "AddBuffEvent");
        Kaiyun.Event.RegisterOut("SpecialActorRemoveBuff", this, "RemoveBuffEvent");
        buffPrefab = Res.LoadAssetSync<GameObject>("UI/Prefabs/Combat/Component/BuffUI.prefab");
    }

    private void OnDestroy()
    {
        Kaiyun.Event.UnRegisterOut("SpecialActorAddBuff", this, "AddBuffEvent");
        Kaiyun.Event.UnRegisterOut("SpecialActorRemoveBuff", this, "RemoveBuffEvent");
    }

    public void SetOwner(Actor actor)
    {
        this.owner = actor;

        foreach(var item in buffUIdict.Values)
        {
            Destroy(item.gameObject);
        }
        buffUIdict.Clear();

        foreach(var buf in actor.BuffManager.GetBuffs())
        {
            AddBuffUI(buf);
        }
    }

    private void Update()
    {
        if (buffUIdict.Count <= 0) return;

        foreach(var item in buffUIdict)
        {
            if (item.Value._buff.CurLevel == 0)
            {

            }
            item.Value.UpdateUI(Time.deltaTime);
        }

    }

    private void AddBuffUI(Buff buff)
    {
        if (buff == null) return;

        //实例化ui，放在当前对象下面
        var obj = Instantiate(buffPrefab, transform);
        var buffui = obj.GetComponent<BuffUIScript>();
        buffui.Init(buff);

        //加入字典中管理
        buffUIdict.TryAdd(buff.InstanceId, buffui);
    }
    private void RemoveBuffUI(int id) {
        if (buffUIdict.ContainsKey(id))
        {
            Destroy(buffUIdict[id].gameObject);
            buffUIdict.Remove(id);
        }
    }

    // event
    public void AddBuffEvent(Buff buff)
    {
        if(owner != null && buff != null &&buff.Owner.EntityId == owner.EntityId)
        {
            AddBuffUI(buff);
        }
    }
    public void RemoveBuffEvent(Actor actor,int id)
    {
        if (owner != null && actor == owner)
        {
            RemoveBuffUI(id);
        }
    }

}
