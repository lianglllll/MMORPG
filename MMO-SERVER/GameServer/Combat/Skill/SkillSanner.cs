﻿
namespace GameServer.Skills
{
    using GameServer.Combat.Skill;
    using GameServer.Model;
    using Serilog;
    using System;
    using System.Collections.Concurrent;
    using System.Reflection;

    [AttributeUsage(AttributeTargets.Class)]
    public class SkillAttribute : Attribute
    {
        //技能码
        public int Code { get; }

        public SkillAttribute(int code)
        {
            this.Code = code;
        }
    }

    public class SkillSanner
    {
        public static ConcurrentDictionary<int, Type> SkillTypeDict = new();

        /// <summary>
        /// 扫描项目里带有[Skill]属性的class
        /// </summary>
        public static void Start()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes();

            Type skillType = typeof(Skill);

            foreach (Type type in types)
            {
                if (Attribute.IsDefined(type, typeof(SkillAttribute)))
                {
                    var attribute = (SkillAttribute)Attribute.GetCustomAttribute(type, typeof(SkillAttribute));
                    int skid = attribute.Code;
                    
                    if (skillType.IsAssignableFrom(type.BaseType))
                    {
                        Log.Information("加载技能类型:Code=[{0}],Name=[{1}]", skid, type.Name);
                        SkillTypeDict[skid] = type;
                    }
                    else
                    {
                        Log.Error("未继承Skill基类:Name=[{1}]", skid, type.Name);
                    }
                    
                }
            }
        }

        /// <summary>
        /// 创建Skill实例
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="skid"></param>
        /// <returns></returns>
        public static Skill CreateSkill(Actor owner, int skid)
        {
            //1.如果有注解则使用所在的类型
            if (SkillTypeDict.TryGetValue(skid, out var skillType))
            {
                object instance = Activator.CreateInstance(skillType, owner, skid);
                return (Skill)instance;
            }
            //2.如果匹配不到则使用基础类型
            return new Skill(owner, skid);
        }
    }

}