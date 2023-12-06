using Serilog;
using Summer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proto;
using GameServer.Manager;
using GameServer.core;
using GameServer.Combat;
using Summer;
using Google.Protobuf;

namespace GameServer.Model
{
    /// <summary>
    /// 空间、地图、场景
    /// </summary>
    public class Space
    {
        public int SpaceId { get; set; }
        public string Name { get; set; }
        public SpaceDefine def { get; set; }

        //当前地图中所有的Character<角色id，角色引用>
        private Dictionary<int, Character> characterDict = new Dictionary<int, Character>();
        private Dictionary<Connection, Character> connCharacter = new Dictionary<Connection, Character>();

        public MonsterManager monsterManager = new MonsterManager();
        public SpawnManager spawnManager = new SpawnManager();
        public FightManager fightManager = new FightManager();



        public void Update()
        {
            spawnManager.Update();
            fightManager.OnUpdate(Time.deltaTime);
        }


        public Space(){}

        public Space(SpaceDefine spaceDefine)
        {
            def = spaceDefine;
            SpaceId = spaceDefine.SID;
            Name = spaceDefine.Name;
            monsterManager.Init(this);
            spawnManager.Init(this);
            fightManager.Init(this);
        }


        //角色进入地图
        public void CharaterJoin(Connection conn, Character character)
        {
            Log.Information("有角色进入场景:"+ SpaceId + ",他的id为：" + character.Id);

            //将角色引用放入conn连接当中
            conn.Get<Session>().character = character;

            character.OnEnterSpace(this);

            //将新加入的character交给当前场景来管理
            characterDict[character.Id] = character;
            character.conn = conn;


            if (!connCharacter.ContainsKey(conn))
            {
                connCharacter[conn] = character;
            }


            //新加入的character广播给场景中的其他玩家(排除自己) 
            var resp = new SpaceCharactersEnterResponse();
            resp.SpaceId = this.SpaceId;
            resp.CharacterList.Add(character.info);
            foreach (var kv in characterDict)
            {
                if (kv.Value.conn != conn)
                {
                    kv.Value.conn.Send(resp);
                }
            }


            //新上线的玩家需要获取场景中全部的角色消息(排除自己) 
            //新上线的玩家需要获取场景中全部的怪物信息
            resp.CharacterList.Clear();
            foreach (var kv in characterDict)
            {
                if (kv.Value.conn == conn) continue;
                resp.CharacterList.Add(kv.Value.info);
            }

            foreach (var kv in monsterManager.monsterDict)
            {
                resp.CharacterList.Add(kv.Value.info);
                conn.Send(resp);
            }
            conn.Send(resp); 

        }


        //角色离开地图(客户端离线、切换地图)
        public void CharacterLeave(Connection conn,Character character)
        {
            Log.Information("角色离开场景，id：" + character.Id);

            characterDict.Remove(character.Id);
            connCharacter.Remove(conn);

            SpaceCharactersLeaveResponse resp = new SpaceCharactersLeaveResponse();
            resp.EntityId = character.EntityId;

            foreach (var kv in characterDict)
            {
                kv.Value.conn.Send(resp);
            }


        }


        /// 更新服务器目标entity的信息，并且给其他玩家进行转发
        /// <param name="entitySync">位置+状态</param>
        public void UpdateEntity(NEntitySync entitySync)
        {
            foreach (var kv in characterDict)
            {
                if (kv.Value.EntityId == entitySync.Entity.Id)
                {
                    kv.Value.EntityData = entitySync.Entity;
                }
                else
                {
                    SpaceEntitySyncResponse resp = new SpaceEntitySyncResponse();
                    resp.EntitySync = entitySync;
                    kv.Value.conn.Send(resp);
                }
            }
        }


        //怪物进入地图
        public void MonsterJoin(Monster monster)
        {
            //修改切换场景后的参数（新创建的，切换地图的）
            monster.OnEnterSpace(this);

            var resp = new SpaceCharactersEnterResponse();
            resp.SpaceId = this.SpaceId;
            resp.CharacterList.Add(monster.info);

            //广播地图内所有玩家
            Broadcast(resp);
        }

        //广播一个proto消息，给场景的全体玩家
        public void Broadcast(IMessage msg)
        {
            foreach(var kv in characterDict)
            {
                kv.Value.conn.Send(msg);
            }
        }


    }
}
