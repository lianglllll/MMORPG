using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Summer
{
    //where T: new() 参数类型约束，T必须要有一个无参构造函数
	/// <summary>
	/// 单例模式基础类
	/// </summary>
	/// <typeparam name="T">泛型</typeparam>
    public class Singleton<T> where T : new()
	{
		//问号代表可空类型
		private static T? instance;
		public static T Instance
		{
			get
			{
				if (instance == null)
				{
                    instance = new T();
				}
				return instance;
			}
		}
	}
}
