using Proto;
using Summer;
using Summer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Database;
using GameServer.Manager;
using Serilog;
using GameServer.Core;

namespace GameServer.Model
{
    /// <summary>
    /// 角色，一个玩家可以选择不同的角色
    /// </summary>
    public class Character:Actor
    {

        //当前角色的客户端
        public Connection conn;

        //当前角色对应的数据库对象信息
        public DbCharacter Data;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbCharacter"></param>
        public Character(DbCharacter dbCharacter) : base(EntityType.Character,dbCharacter.JobId,dbCharacter.Level,new Vector3Int(dbCharacter.X, dbCharacter.Y, dbCharacter.Z), Vector3Int.zero)
        {

            //将角色信息转换为Character
            this.Id = dbCharacter.Id;
            this.Name = dbCharacter.Name;
            this.Data = dbCharacter;

            this.info.Name = dbCharacter.Name;              //覆盖
            this.info.Hp = dbCharacter.Hp;                  //覆盖
            this.info.Mp = dbCharacter.Mp;                  //覆盖

            this.info.Id = dbCharacter.Id;                  //独有
            this.info.Exp = dbCharacter.Exp;                //独有
            this.info.SpaceId = dbCharacter.SpaceId;        //
            this.info.Gold = dbCharacter.Gold;              //独有

            //this.info.Entity;      使用时需要自动调用entitydata进行赋值
            //this.info.Entity.Id;
            //this.EntityId;        entityid 等待entitymanager分配id

        }

        /// <summary>
        /// 重载运算符=,隐式类型转换
        /// </summary>
        /// <param name="dbCharacter"></param>
        public static implicit operator Character(DbCharacter dbCharacter)
        {
            return new Character(dbCharacter);
        }

    }


}
