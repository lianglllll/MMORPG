﻿using Serilog;
using Common.Summer.Core;
using Serilog.Sinks.SystemConsole.Themes;
using ControlCenter.Utils;
using ControlCenter.Net;
using ControlCenter.Core;

namespace ControlCenter
{
    class Program
    {
        private static bool Init()
        {
            //初始化日志环境
            var customTheme = new AnsiConsoleTheme(new Dictionary<ConsoleThemeStyle, string>
            {
                [ConsoleThemeStyle.Text] = "\x1b[37m", // White
                [ConsoleThemeStyle.SecondaryText] = "\x1b[37m", // Gray
                [ConsoleThemeStyle.TertiaryText] = "\x1b[90m", // Dark gray
                [ConsoleThemeStyle.Invalid] = "\x1b[33m", // Yellow
                [ConsoleThemeStyle.Null] = "\x1b[34m", // Blue
                [ConsoleThemeStyle.Name] = "\x1b[32m", // Green
                [ConsoleThemeStyle.String] = "\x1b[36m", // Cyan
                [ConsoleThemeStyle.Number] = "\x1b[35m", // Magenta
                [ConsoleThemeStyle.Boolean] = "\x1b[34m", // Blue
                [ConsoleThemeStyle.Scalar] = "\x1b[32m", // Green
                [ConsoleThemeStyle.LevelVerbose] = "\x1b[90m", // Dark gray
                [ConsoleThemeStyle.LevelDebug] = "\x1b[37m", // White
                [ConsoleThemeStyle.LevelInformation] = "\x1b[32m", // Green
                [ConsoleThemeStyle.LevelWarning] = "\x1b[33m", // Yellow
                [ConsoleThemeStyle.LevelError] = "\x1b[31m", // Red
                [ConsoleThemeStyle.LevelFatal] = "\x1b[41m\x1b[37m" // Red background, white text
            });
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(
                    theme: customTheme,
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
                )
                .WriteTo.File(
                    "logs\\server-log.txt",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                )
                .CreateLogger();

            Config.Init();
            Scheduler.Instance.Start(Config.Server.updateHz);
            ServersMgr.Instance.Init();
            NetService.Instance.Init();

            Log.Information("[ControlCenter]初始化成功,配置如下：");
            Log.Information($"ip：{Config.Server.ip}");
            Log.Information($"port：{Config.Server.port}");
            Log.Information($"workerCount：{Config.Server.workerCount}");
            Log.Information($"updateHz：{Config.Server.updateHz}");
            return true;
        }
        private static bool UnInit()
        {
            return true;
        }
        private static bool Shell()
        {
            while (true)
            {
                Console.Write("press command to execute:\n");
                string input = Console.ReadLine();

                if (string.IsNullOrEmpty(input))
                    continue;

                // Parse command
                string[] args = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                string command = args[0].ToLower();

                switch (command)
                {
                    case "exit":
                        Console.WriteLine("Exiting the shell.");
                        UnInit();
                        Environment.Exit(0);
                        return true;
                    case "showgameserver":
                        var dict = ServersMgr.Instance.GetServers();
                        foreach (var pair in dict)
                        {
                            Console.WriteLine(pair.Key);
                            Console.WriteLine(pair.Value);
                        }
                        break;
                    default:
                        Console.WriteLine($"Unknown command: {command}");
                        break;
                }
            }
        }
        public static void Main(string[] args)
        {
            Init();
            Shell();
        }
    }
}