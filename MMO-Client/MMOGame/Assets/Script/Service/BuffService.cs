using Summer;
using Summer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proto;

namespace Assets.Script.Service
{
    public class BuffService : Singleton<BuffService>, IDisposable
    {
        public void Init()
        {
            MessageRouter.Instance.Subscribe<BuffsAddResponse>(_BuffsAddResponse);
            MessageRouter.Instance.Subscribe<BuffsRemoveResponse>(_BuffsRemoveResponse);
            MessageRouter.Instance.Subscribe<BuffsUpdateResponse>(_BuffsUpdateResponse);
        }
        public void Dispose()
        {
            MessageRouter.Instance.Off<BuffsAddResponse>(_BuffsAddResponse);
            MessageRouter.Instance.Off<BuffsRemoveResponse>(_BuffsRemoveResponse);
            MessageRouter.Instance.Off<BuffsUpdateResponse>(_BuffsUpdateResponse);
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
                var actor = GameTools.GetUnit(info.OwnerId);
                if (actor == null) continue;
                actor.RemoveBuff(info.Id);
            }
        }


        private void _BuffsUpdateResponse(Connection sender, BuffsUpdateResponse msg)
        {

        }

    }
}
