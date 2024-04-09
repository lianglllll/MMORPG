using Summer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Threading;
using Common;
using Serilog;

namespace Summer.Network
{

    class Msg
    {
        public Connection sender;
        public Google.Protobuf.IMessage message;
    }

    /// <summary>
    /// 消息分发器
    /// </summary>
    public class MessageRouter : Singleton<MessageRouter>
    {

        int ThreadCount = 1;    //工作线程数
        int WorkerCount = 0;    //正在工作的线程数
        bool _running = false;   //是否正在运行状态

        public bool Running { get { return _running; } }

        AutoResetEvent threadEvent = new AutoResetEvent(true); //通过Set每次可以唤醒1个线程

        // 消息队列，所有客户端发来的消息都暂存在这里
        private Queue<Msg> messageQueue = new Queue<Msg>();
        // 消息处理器(委托)
        public delegate void MessageHandler<T>(Connection sender, T msg);
        // 频道字典（订阅记录）
        private Dictionary<string, Delegate> delegateMap = new Dictionary<string, Delegate>();


        //订阅
        public void Subscribe<T>(MessageHandler<T> handler) where T : Google.Protobuf.IMessage
        {
            string type = typeof(T).FullName;
            if (!delegateMap.ContainsKey(type))
            {
                delegateMap[type] = null;
            }
            delegateMap[type] = (MessageHandler<T>)delegateMap[type] + handler;
            //Log.Debug(type+":"+delegateMap[type].GetInvocationList().Length);
        }
        //退订
        public void Off<T>(MessageHandler<T> handler) where T : Google.Protobuf.IMessage
        {
            string key = typeof(T).FullName;
            if (!delegateMap.ContainsKey(key))
            {
                delegateMap[key] = null;
            }
            delegateMap[key] = (MessageHandler<T>)delegateMap[key] - handler;
        }

        //触发
        private void Fire<T>(Connection sender, T msg)
        {
            string type = typeof(T).FullName;
            if (delegateMap.ContainsKey(type))
            {
                MessageHandler<T> handler = (MessageHandler<T>)delegateMap[type];
                try
                {
                    handler?.Invoke(sender, msg);
                }
                catch(Exception e)
                {
                    Log.Error("MessageRouter.Fire error:" + e.StackTrace);
                }
                
            }
        }


        /// <summary>
        /// 添加新的消息到队列中
        /// </summary>
        /// <param name="sender">消息发送者</param>
        /// <param name="message">消息对象</param>
        public void AddMessage(Connection sender, Google.Protobuf.IMessage message)
        {
            lock (messageQueue)
            {
                messageQueue.Enqueue(new Msg() { sender = sender, message = message });
            }
            threadEvent.Set(); //唤醒1个worker
        }


        public void Start(int _ThreadCount)
        {
            if (_running) return;
            _running = true;
            ThreadCount = Math.Min(Math.Max(_ThreadCount, 1), 8);
            ThreadPool.SetMinThreads(ThreadCount+20, ThreadCount+20);
            for (int i = 0; i < ThreadCount; i++)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(MessageWork));
            }

            //等待全部线程创建完毕！
            while(WorkerCount< ThreadCount)
            {
                Thread.Sleep(100);
            }
        }

        public void Stop()
        {
            _running = false;
            messageQueue.Clear();
            while (WorkerCount > 0)
            {
                threadEvent.Set();
            }
            Thread.Sleep(100);
        }


        private void MessageWork(object? state)
        {
            Log.Information("worker thread start");
            try
            {
                Interlocked.Increment(ref WorkerCount);
                while (_running)
                {
                    if (messageQueue.Count == 0)
                    {
                        threadEvent.WaitOne(); //可以通过Set()唤醒
                        continue;
                    }
                    //从消息队列取出一个元素
                    Msg msg = null;
                    lock (messageQueue)
                    {
                        if (messageQueue.Count == 0) continue;
                        msg = messageQueue.Dequeue();
                    }
                    Google.Protobuf.IMessage package = msg.message;
                    if(package != null)
                    {
                        executeMessage(msg.sender, package);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            finally
            {
               Interlocked.Decrement(ref WorkerCount);
            }
            Log.Information("worker thread end");
        }

        //递归处理消息
        private void executeMessage(Connection conn, Google.Protobuf.IMessage message)
        {
            //触发订阅
            var fireMethod = this.GetType().GetMethod("Fire", BindingFlags.NonPublic | BindingFlags.Instance);
            var met = fireMethod.MakeGenericMethod(message.GetType());
            met.Invoke(this, new object[] { conn, message });


            //可弃用
/*            var t = message.GetType();
            foreach (var p in t.GetProperties())
            {
                //过滤属性
                if ("Parser" == p.Name || "Descriptor" == p.Name) continue;
                var value = p.GetValue(message);
                if (value != null)
                {
                    if (typeof(Google.Protobuf.IMessage).IsAssignableFrom(value.GetType()))
                    {
                        //继续递归
                        executeMessage(conn, (Google.Protobuf.IMessage)value);
                    }
                }
            }*/
        }


    }

}
