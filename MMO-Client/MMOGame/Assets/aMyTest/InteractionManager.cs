using Summer;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 记录所有的交互行为
/// 每个场景只有一个这个管理器
/// </summary>
public class InteractionManager : Singleton<InteractionManager>
{

    private int idGenerator = 0;
    private ConcurrentDictionary <int, IInteraction> interactionDict = new();

    public int AddInteraciton(IInteraction interaction)
    {
        if (interaction == null) return 0;
        var id = ++idGenerator;
        interactionDict.TryAdd(id, interaction);
        return id;
    }

    public IInteraction RemoveInteraction(int id)
    {
        interactionDict.TryRemove(id, out var interaction);
        return interaction;
    }

    public IInteraction GetInteration(int id)
    {
        interactionDict.TryGetValue(id, out var interaction);
        return interaction;
    }


}
