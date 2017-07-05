using System;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerLogger.Init(args[0]); // 初始化日志系统

            // 构建并启动服务器
            Server srv = new Server();
            ServerBuilder.BuildGameServer(srv, "127.0.0.1", 9600);
            srv.Start();
        }
    }
}
