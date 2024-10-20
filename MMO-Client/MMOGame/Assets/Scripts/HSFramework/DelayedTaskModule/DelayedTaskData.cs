using System;

namespace BaseSystem.MyDelayedTaskScheduler
{
    public class DelayedTaskData
    {
        public long Time;
        public string Token;
        public Action Action;
        public Action EarlyRemoveCallback;
    }
}
