using Common.Summer.Net;
using Common.Summer.Core;
using HS.Protobuf.Combat.Buff;
using BaseSystem.Tool.Singleton;
using GameClient.Combat.Buffs;

namespace Assets.Script.Service
{
    public class BuffService : SingletonNonMono<BuffService>
    {
        public void Init()
        {
            MessageRouter.Instance.Subscribe<BuffsAddResponse>(HandleBuffsAddResponse);
            MessageRouter.Instance.Subscribe<BuffsRemoveResponse>(HandleBuffsRemoveResponse);
            MessageRouter.Instance.Subscribe<BuffsUpdateResponse>(HandleBuffsUpdateResponse);
        }
        public void UnInit()
        {
            MessageRouter.Instance.UnSubscribe<BuffsAddResponse>(HandleBuffsAddResponse);
            MessageRouter.Instance.UnSubscribe<BuffsRemoveResponse>(HandleBuffsRemoveResponse);
            MessageRouter.Instance.UnSubscribe<BuffsUpdateResponse>(HandleBuffsUpdateResponse);
        }

        private void HandleBuffsAddResponse(Connection sender, BuffsAddResponse msg)
        {
            foreach(var info in msg.List)
            {
                var buff = new Buff();
                buff.Init(info);
                if (buff.Owner == null) continue;
            }
        }
        private void HandleBuffsRemoveResponse(Connection sender, BuffsRemoveResponse msg)
        {
            foreach (var info in msg.List)
            {
                var actor = GameTools.GetActorById(info.OwnerId);
                if (actor == null) continue;
                actor.BuffManager.RemoveBuff(info.Id);
            }
        }
        private void HandleBuffsUpdateResponse(Connection sender, BuffsUpdateResponse msg)
        {

        }
    }
}
