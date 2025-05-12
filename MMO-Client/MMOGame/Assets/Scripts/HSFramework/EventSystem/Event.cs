using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;


namespace Kaiyun
{
    /// <summary>
    /// 事件系统,观察者模式,私有方法反射出来用不了？
    /// </summary>
    /// <version>1.2</version>
    /// <date>2023-08-02</date>
    public class Event
    {

        private static Dictionary<string, List<GsHandler>> eventInDict;
        private static Dictionary<string, List<GsHandler>> eventOutDict;

        private static Queue<FireTask> outQueue;

        public delegate void EventAction(params object[] args);

        private class FireTask
        {
            public string name; //事件名称
            public object[] args;
            public FireTask(string name, object[] args)
            {
                this.name = name;
                this.args = args;
            }
        }



        private class GsHandler
        {
            public object target;
            public string methodName;
            public EventAction action;
            public GsHandler(object target, string methodName)
            {
                this.target = target;
                this.methodName = methodName;
                MethodInfo method = target.GetType().GetMethod(methodName);
                if (method != null)
                {
                    action = args => method.Invoke(target, args);
                }

            }
        }

        static Event()
        {
            eventInDict = new Dictionary<string, List<GsHandler>>();
            eventOutDict = new Dictionary<string, List<GsHandler>>();
            outQueue = new Queue<FireTask>();
        }

        //用于不涉及主线程的操作
        //参数：事件名称  触发事件的目标    方法
        public static void RegisterIn(string eventName, object target, string methodName)
        {
            lock (eventInDict)
            {
                if (!eventInDict.ContainsKey(eventName))
                {
                    eventInDict[eventName] = new List<GsHandler>();
                }
                eventInDict[eventName].Add(new GsHandler(target, methodName));
            }

        }

        //用于涉及主线程的操作
        public static void RegisterOut(string eventName, object target, string methodName)
        {
            lock (eventOutDict)
            {
                if (!eventOutDict.ContainsKey(eventName))
                {
                    eventOutDict[eventName] = new List<GsHandler>();
                }
                eventOutDict[eventName].Add(new GsHandler(target, methodName));
            }

        }


        //事件触发
        public static void FireIn(string eventName, params object[] parameters)
        {
            lock (eventInDict)
            {
                if (eventInDict.ContainsKey(eventName))
                {
                    List<GsHandler> list = eventInDict[eventName];
                    foreach (GsHandler handler in list)
                    {
                        handler.action?.Invoke(parameters);
                    }
                }
            }

        }

        public static void FireOut(string eventName, params object[] parameters)
        {
            lock (eventOutDict)
            {
                if (eventOutDict.ContainsKey(eventName))
                {
                    outQueue.Enqueue(new FireTask(eventName, parameters));
                }
            }
        }

        public static void UnRegisterIn(string eventName, object target, string methodName)
        {
            lock (eventInDict)
            {
                var list = eventInDict.GetValueOrDefault(eventName, null);
                list?.RemoveAll(h => h.target == target && h.methodName.Equals(methodName));
            }
        }

        public static void UnRegisterOut(string eventName, object target, string methodName)
        {
            lock (eventOutDict)
            {
                var list = eventOutDict.GetValueOrDefault(eventName, null);
                list?.RemoveAll(h => h.target == target && h.methodName.Equals(methodName));
            }
        }


        public static void UnregisterIn(string eventName)
        {
            lock (eventInDict)
            {
                eventInDict.Clear();
            }
        }
        public static void UnregisterOut(string eventName)
        {
            lock (eventOutDict)
            {
                eventOutDict.Clear();
            }
        }


        /// <summary>
        /// 在主线程Update调用
        /// </summary>
        public static void Tick()
        {
            if (System.Threading.Thread.CurrentThread.ManagedThreadId == 1)
            {
                // 当前代码在主线程中运行
                // Debug.Log("主线程");
                while (outQueue.Count > 0)
                {
                    var item = outQueue.Dequeue();
                    var list = eventOutDict.GetValueOrDefault(item.name, null);
                    foreach (var handler in list)
                    {
                        handler.action?.Invoke(item.args);
                    }
                }
            }

        }


    }

}
