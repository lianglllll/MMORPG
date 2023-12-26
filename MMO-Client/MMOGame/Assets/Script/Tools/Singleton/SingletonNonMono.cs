using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGG.Tool.Singleton
{
    public abstract class SingletonNonMono<T> where T : class,new()
    {
        private static T _instance;
        private static object _lock = new object();

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new T();
                    }
                }
                return _instance;
            }
        }
    }

}