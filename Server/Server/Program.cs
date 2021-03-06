﻿using System;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerLogger.Init(args[0]); // 初始化日志系统

            // 构建服务器
            Server srv = new Server();
            ServerBuilder.BuildGameServer(srv, "0.0.0.0", int.Parse(args[1]));

            // 启动服务器
            srv.Start();
        }
    }
}
