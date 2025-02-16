using System;
using System.Collections.Generic;

namespace HSFramework.AI.StateMachine
{
    public interface IStateMachineOwner { }

    public class StateMachine
    {
        private IStateMachineOwner owner;
        private StateBase curState;
        private Dictionary<Type, StateBase> stateDict = new();
        public StateBase CurState { get => curState; }

        public void Init(IStateMachineOwner owner)
        {
            this.owner = owner;
        }
        public void UnInit()
        {
            if (curState != null)
            {
                curState.Exit();
                MonoManager.Instance.RemoveUpdateListener(curState.Update);
                MonoManager.Instance.RemoveFixedUpdateListener(curState.FixedUpdate);
                MonoManager.Instance.RemoveLateUpdateListener(curState.LateUpdate);
            }
        }
        public void Stop()
        {
            curState.Exit();
            MonoManager.Instance.RemoveUpdateListener(curState.Update);
            MonoManager.Instance.RemoveFixedUpdateListener(curState.FixedUpdate);
            MonoManager.Instance.RemoveLateUpdateListener(curState.LateUpdate);
            curState = null;

            foreach (var item in stateDict.Values)
            {
                item.UnInit();
            }
            stateDict.Clear();
        }
        private StateBase GetState<T>() where T : StateBase, new()
        {
            Type type = typeof(T);
            if (!stateDict.TryGetValue(type, out var state))
            {
                state = new T();
                state.Init(owner);
                stateDict.Add(type, state);
            }
            return state;
        }
        public bool ChangeState<T>(bool reCurrstate = false) where T : StateBase, new()
        {
            if (curState != null && curState.GetType() == typeof(T) && !reCurrstate) return false;

            //退出当前状态
            if (curState != null)
            {
                curState.Exit();
                MonoManager.Instance.RemoveUpdateListener(curState.Update);
                MonoManager.Instance.RemoveFixedUpdateListener(curState.FixedUpdate);
                MonoManager.Instance.RemoveLateUpdateListener(curState.LateUpdate);
            }

            //进入新状态
            curState = GetState<T>();
            curState.Enter();
            MonoManager.Instance.AddUpdateListener(curState.Update);
            MonoManager.Instance.AddFixedUpdateListener(curState.FixedUpdate);
            MonoManager.Instance.AddLateUpdateListener(curState.LateUpdate);

            return false;
        }
    }
}
