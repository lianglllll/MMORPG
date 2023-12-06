using System;
using System.Net;
using System.Net.Sockets;
using Summer;
using GameServer.Network;
using Summer.Network;
using Proto;
using Serilog;
using GameServer.Service;
using GameServer.Database;
using GameServer.Model;
using GameServer.Manager;
using GameServer.AI;

namespace GameServer
{
    class Program
    {
        static void Main(string[] args)
        {

            //初始化日志环境
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File("logs\\server-log.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();
            Log.Debug("[日志服务启动完成]");

            Log.Debug("装载消息类型=>");
            ProtoHelper.Init();

            //装载配置文件
            DataManager.Instance.init();
            Log.Debug("[Json配置文件加载完成]");

            //开启网络服务
            NetService netService = new NetService();
            netService.Start();
            Log.Debug("[网络服务启动完成]");

            //开启玩家服务
            UserService.Instance.Start();
            Log.Debug("[玩家服务启动完成]");

            //开启地图服务
            SpaceService.Instance.Start();
            Log.Debug("[地图服务启动完成]");

            //开启战斗服务
            CombatService.Instance.Start();
            Log.Debug("[战斗服务启动完成]");


            //中心计时器任务加载
            Scheduler.Instance.Start();
            Log.Debug("[中心计时器任务加载完成]");

            //中心计时器任务：
            Scheduler.Instance.AddTask(() => {
                EntityManager.Instance.Update();
                SpaceManager.Instance.Update();
            }, 0.02f);


            //服务器启动完毕，开始工作
            Log.Debug("[mmorpg服务器启动成功]");

            //test
            Space space = SpaceService.Instance.GetSpaceById(1);
            Monster mon =  space.monsterManager.Create(1, 100, new Core.Vector3Int(150000, 0, 150000), Core.Vector3Int.zero);
            mon.AI = new MonsterAI(mon);


            Console.ReadKey();//防止进程结束
        }

    }
}
 