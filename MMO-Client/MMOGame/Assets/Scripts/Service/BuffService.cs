using Summer;
using Common.Summer.Net;
using Common.Summer.Core;
using HS.Protobuf.Combat.Buff;

namespace Assets.Script.Service
{
    public class BuffService : Singleton<BuffService>
    {
        public void Init()
        {
            MessageRouter.Instance.Subscribe<BuffsAddResponse>(_BuffsAddResponse);
            MessageRouter.Instance.Subscribe<BuffsRemoveResponse>(_BuffsRemoveResponse);
            MessageRouter.Instance.Subscribe<BuffsUpdateResponse>(_BuffsUpdateResponse);
        }
        public void UnInit()
        {
            MessageRouter.Instance.UnSubscribe<BuffsAddResponse>(_BuffsAddResponse);
            MessageRouter.Instance.UnSubscribe<BuffsRemoveResponse>(_BuffsRemoveResponse);
            MessageRouter.Instance.UnSubscribe<BuffsUpdateResponse>(_BuffsUpdateResponse);
        }

        private void _BuffsAddResponse(Connection sender, BuffsAddResponse msg)
        {
            foreach(var info in msg.List)
            {
                var buff = new Buff();
                buff.Init(info);
                if (buff.Owner == null) continue;

            }
        }


        private void _BuffsRemoveResponse(Connection sender, BuffsRemoveResponse msg)
        {
            foreach (var info in msg.List)
            {
                var actor = GameTools.GetActorById(info.OwnerId);
                if (actor == null) continue;
                actor.RemoveBuff(info.Id);
            }
        }


        private void _BuffsUpdateResponse(Connection sender, BuffsUpdateResponse msg)
        {

        }

    }
}
