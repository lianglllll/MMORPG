﻿using GameServer.Core;
using GameServer.Manager;
using GameServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.core
{
    public class GameTools
    {


        /// <summary>
        /// 根据entityId获取一个actor
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public static Actor GetUnit(int entityId)
        {
            return EntityManager.Instance.GetEntity(entityId) as Actor;
        }

        /// <summary>
        /// 返回指定范围内的entity
        /// </summary>
        /// <param name="spaceId"></param>
        /// <param name="position"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static List<Actor> RangUnit(int spaceId, Vector3 position,int range)
        {
            Predicate<Actor> match = (e) =>
            {
                return Vector3Int.Distance(position, e.Position) <= range;
            };
            return EntityManager.Instance.GetEntityList(spaceId, match);
        }



    }
}
