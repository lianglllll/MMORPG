using System;

namespace HSFramework.MyDelayedTaskScheduler
{
    public static class TimerUtil
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <param name="isMillisecond">是否毫秒</param>
        /// <returns>当前时间戳</returns>
        public static long GetTimeStamp(bool isMillisecond = false)
        {
            return isMillisecond ? (long)(DateTime.UtcNow - UnixEpoch).TotalMilliseconds : (long)(DateTime.UtcNow - UnixEpoch).TotalSeconds;
        }
        
        public static long GetLaterMilliSecondsBySecond(double time)
        {
            return (long)TimeSpan.FromMilliseconds(GetTimeStamp(true)).Add(TimeSpan.FromSeconds(time))
                .TotalMilliseconds;
        }
    }
}
