using Serilog;
using Serilog.Sinks.Unity3D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerilogConfig : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake()
    {
        // 设置 Serilog 配置
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Unity3D()
            .CreateLogger();
    }

    void OnDestroy()
    {
        // 关闭 Serilog 日志记录器
        Log.CloseAndFlush();
    }
}
