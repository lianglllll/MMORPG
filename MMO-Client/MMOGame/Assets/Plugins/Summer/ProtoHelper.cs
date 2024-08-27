using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using System.IO;
using System;
using System.Reflection;
using Google.Protobuf.Reflection;
using Serilog;
using System.Linq;

namespace Summer
{
    /// <summary>
    /// Protobuf序列化与反序列化
    /// </summary>
    public class ProtoHelper
    {
        /// <summary>
        /// 序列化protobuf
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static byte[] Serialize(IMessage msg)
        {
            using (MemoryStream rawOutput = new MemoryStream())
            {
                msg.WriteTo(rawOutput);
                byte[] result = rawOutput.ToArray();
                return result;
            }
        }

        /// <summary>
        /// 解析
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataBytes"></param>
        /// <returns></returns>
        public static T Parse<T>(byte[] dataBytes) where T : IMessage, new()
        {
            T msg = new T();
            msg = (T)msg.Descriptor.Parser.ParseFrom(dataBytes);
            return msg;
        }

        private static Dictionary<string, Type> _registry = new Dictionary<string, Type>();
        private static Dictionary<int, Type> mDict1 = new Dictionary<int, Type>();
        private static Dictionary<Type, int> mDict2 = new Dictionary<Type,int>();

        static ProtoHelper()
        {
            List<string> list = new List<string>();
            // 查找并获取指定名称的程序集
            var tpyes = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == "Assembly-CSharp")
                .GetTypes();

            tpyes.ToList().ForEach(t =>
            {
                if (typeof(IMessage).IsAssignableFrom(t))
                {
                    var desc = t.GetProperty("Descriptor").GetValue(t) as MessageDescriptor;
                    _registry.Add(desc.FullName, t);
                    list.Add(desc.FullName);
                    
                }
            });

            list.Sort((x,y)=>{
                //按照字符串长度排序，
                if (x.Length != y.Length)
                {
                    return x.Length - y.Length;
                }
                //如果长度相同
                return string.Compare(x, y, StringComparison.Ordinal);
            });
            
            for (int i = 0; i < list.Count; i++)
            {
                var fname = list[i];
                var t = _registry[fname];
                //Log.Debug("Proto类型注册：{0} - {1}",i, fname);
                mDict1[i] = t;
                mDict2[t] = i;
            }
            
        }

        public static int SeqCode(Type type)
        {
            return mDict2[type];
        }
        public static Type SeqType(int code)
        {
            return mDict1[code];
        }


        /// <summary>
        /// 根据消息编码进行解析
        /// </summary>
        /// <param name="typeCode"></param>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static IMessage ParseFrom(int typeCode, byte[] data,int offset,int len)
        {
            Type t = ProtoHelper.SeqType(typeCode);
            var desc = t.GetProperty("Descriptor").GetValue(t) as MessageDescriptor;
            var msg = desc.Parser.ParseFrom(data, offset, len);
            //Log.Information("解析消息：code={0} - {1}", typeCode, msg);
            return msg;
        }


    }
}
