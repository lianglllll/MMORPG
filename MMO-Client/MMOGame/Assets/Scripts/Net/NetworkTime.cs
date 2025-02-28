using HSFramework.MySingleton;
using System;

public class NetworkTime : Singleton<NetworkTime>
{
    public long GetCurNetWorkTime()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}
