using FreeSql;
using GameServer.Database;
using GameServer.Model;
using Summer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Manager
{


    //统一管理全部的角色（创建，移除，获取）
    public class CharacterManager: Singleton<CharacterManager>
    {
        //游戏中全部的角色<characterId,characterObj>,支持线程安全的字典
        private ConcurrentDictionary<int, Character> characterDict = new ConcurrentDictionary<int, Character>();

        //角色表的数据库对象
        IBaseRepository<DbCharacter> repo = DbManager.fsql.GetRepository<DbCharacter>();
        


        public CharacterManager() {
            //计时任务，用于保存角色信息
            //5秒保存一次
            Scheduler.Instance.AddTask(SaveCharacterInfo, 5);
        }


        //创建的同时，记录在manager中
        public Character CreateCharacter(DbCharacter dbchr)
        {
            Character chr = new Character(dbchr);
            EntityManager.Instance.AddEntity(dbchr.SpaceId, chr);
            characterDict[chr.Id] = chr;
            return chr;
        }

        public void RemoveCharacter(int chrId)
        {
            Character chr;
            if(characterDict.TryRemove(chrId,out chr)){
                EntityManager.Instance.RemoveEntity(chr.currentSpace.SpaceId, chr);
            }
        }

        public Character GetCharacter(int chrId)
        {
            return characterDict.GetValueOrDefault(chrId, null);//不存在就返回null
        }


        public void ClearCharacters()
        {
            characterDict.Clear();
        }



        //目前主要用于保存角色位置//todo
        private void SaveCharacterInfo()
        {
            foreach (var chr in characterDict.Values)
            {
                //更新位置
                chr.Data.X = chr.Position.x;
                chr.Data.Y = chr.Position.y;
                chr.Data.Z = chr.Position.z;
                //保存进入数据库
                repo.UpdateAsync(chr.Data);//异步更新
            }
        }

    }
}
