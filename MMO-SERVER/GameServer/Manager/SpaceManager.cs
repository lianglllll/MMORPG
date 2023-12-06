using GameServer.Model;
using Serilog;
using Summer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Manager
{
    public class SpaceManager:Singleton<SpaceManager>
    {

        //地图字典
        private Dictionary<int, Space> spaceDict = new Dictionary<int, Space>();


        public void init()
        {
            //获取datamanager中的spacedefin数据，将其加载到spacemanager中
            foreach(var kv in DataManager.Instance.spaceDefineDict)
            {
                spaceDict[kv.Key] = new Space(kv.Value);
                Log.Information("初始化地图：{0}", kv.Value.Name);
            }
        }

        public Space GetSpaceById(int spaceId)
        {
            if (spaceDict.ContainsKey(spaceId))
            {
                return spaceDict[spaceId];
            }
            return null;
        }

        public void Update()
        {
            foreach(var s in spaceDict.Values)
            {
                s.Update();
            }
        }
    }
}
