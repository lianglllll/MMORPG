﻿using System;
using System.Numerics;

namespace AOI.Old
{
    public static class Test
    {
        public static void Run()
        {
            var aoi = new AoiComponent();
            
            var role1 = aoi.Enter(1, 12, 8);
            
            Console.WriteLine($"玩家一ID:{role1.Id}");
            
            var role2 = aoi.Enter(2, 12, 8);
            
            Console.WriteLine($"玩家二ID:{role2.Id}");
            
            aoi.Update(2, new Vector2(1, 1), 13, 8);  // 玩家二移动
            
            Console.WriteLine($"玩家二周围列表");
            
            foreach (var aoiNode in role2.AoiInfo.MovesSet)
            {
                Console.WriteLine(aoi.GetNode(aoiNode).Position);
            }
            
            Console.WriteLine($"玩家二进入列表");
            
            foreach (var aoiNode in role2.AoiInfo.EntersSet)
            {
                Console.WriteLine(aoi.GetNode(aoiNode).Position);
            }
            
            Console.WriteLine($"玩家二离开列表");
            
            foreach (var aoiNode in role2.AoiInfo.LeavesSet)
            {
                Console.WriteLine(aoi.GetNode(aoiNode).Position);
            }
            
            Console.WriteLine($"玩家二移动列表");
            
            foreach (var aoiNode in role2.AoiInfo.MoveOnlySet)
            {
                Console.WriteLine(aoi.GetNode(aoiNode).Position);
            }
        }
    }
}