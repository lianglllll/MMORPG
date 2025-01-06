using BaseSystem.Singleton;
using System;


//mono托管
public class MonoManager : Singleton<MonoManager>
{
    private Action updateAction;
    private Action lateUpdateAction;
    private Action fixedUpdateAction;

    public void AddUpdateListener(Action action)
    {
        updateAction += action;
    }
    public void RemoveUpdateListener(Action action)
    {
        updateAction -= action;
    }
    public void AddFixedUpdateListener(Action action)
    {
        fixedUpdateAction += action;
    }
    public void RemoveFixedUpdateListener(Action action)
    {
        fixedUpdateAction -= action;
    }
    public void AddLateUpdateListener(Action action)
    {
        lateUpdateAction += action;
    }
    public void RemoveLateUpdateListener(Action action)
    {
        lateUpdateAction -= action;
    }

    private void Update()
    {
        updateAction?.Invoke();
    }
    private void FixedUpdate()
    {
        fixedUpdateAction?.Invoke();
    }
    private void LateUpdate()
    {
        lateUpdateAction?.Invoke();
    }


}

