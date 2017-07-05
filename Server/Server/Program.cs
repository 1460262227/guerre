using System;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerLogger.Init(args[0]); // 初始化日志系统

            // 构建服务器
            Server srv = new Server();
            ServerBuilder.BuildGameServer(srv, "127.0.0.1", 9600);

            // 创建测试房间
            BuildTestRoom(srv);

            // 启动服务器
            srv.Start();
        }

        static void BuildTestRoom(Server srv)
        {
            var grMgr = srv.Get<GameRoomManager>();
            grMgr.CreateNewRoom("test");
        }
    }
}
