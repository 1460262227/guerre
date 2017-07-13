using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Swift;
using Server;
using Guerre;
using Swift.Math;

namespace Server
{
    /// <summary>
    /// 对给定的服务器对象进行执行的构建和装配工作
    /// </summary>
    public static class ServerBuilder
    {
        const int MS4OneMinutes = 60000;

        // 构建一个实验室内用的服务器
        public static void BuildGameServer(Server srv, string ip, int port)
        {
            // 服务器崩溃异常日志
            var log = new ServerLogger(typeof(ServerBuilder));

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(HandleAllUnhandledException);

            // 网络模块
            var nc = new NetCore();
            srv.Add("NetCore", nc);

            // 会话容器
            var sc = new SessionContainer();

            // 玩家管理
            var p = new MySqlDbPersistence<Player, string>(
                "guerre", "localhost", "root", "123456", "players",
                "CREATE TABLE players(ID VARCHAR(100) BINARY, Data MediumBlob, PRIMARY KEY(ID ASC));",
                null, null);
            srv.Add("PlayerPersistence", p);
            var pc = new PlayerContainer(p);
            srv.Add("PlayerContainer", pc);

            // 登录管理
            BuildLoginModule(srv, sc, pc);

            // 房间管理
            BuildGameRoomModule(srv, sc);

            // 碰撞模块
            BuildCollisionModule(srv, sc);

            // 所有初始化完成后，启动服务器的网络监听
            log.Info("server started at port: " + port);
            nc.StartListening(ip, port);
        }

        // 处理所有未被处理的异常信息
        static void HandleAllUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var log = new ServerLogger(typeof(UnhandledExceptionEventArgs));
            try
            {
                // 已经没办法恢复了，只能死在这
                log.Error("Terminating " + e.IsTerminating.ToString());
                log.Error("ExceptionObject " + e.ExceptionObject == null ? " null " : e.ExceptionObject.ToString());
            }
            catch (Exception ex)
            {
                log.Error("Exception in HandleAllUnHandledException: " + ex.Message);
            }
        }

        // 发送消息给指定的玩家
        static void SendTo(SessionContainer sc, string id, string op, Action<IWriteableBuffer> fun)
        {
            var s = sc.Get(id);
            if (s == null)
                return;

            var conn = s.Connection;
            if (conn == null || !conn.IsConnected)
                return;

            var buff = conn.BeginSend("MessageHandler");
            buff.Write(op);
            if (fun != null)
                fun(buff);

            conn.End(buff);
        }

        // 游戏房间模块
        static void BuildGameRoomModule(Server srv, SessionContainer sc)
        {
            var grc = new GameRoomContainer();
            var grMgr = new GameRoomManager();
            grMgr.GRC = grc;
            grMgr.SC = sc;
            srv.Add("GameRoomMgr", grMgr);

            // add a test room
            var gr = grMgr.CreateNewRoom("test");
            gr.SafeAreaLeftTop = new Vec2(-7.5f, -5);
            gr.SafeAreaSize = new Vec2(15, 10);
            var ig = new ItemGenerator();
            ig.GenSpaceLeftTop = new Vec2(-7.5f, -5);
            ig.GenSpaceSize = new Vec2(15, 10);
            ig.MedicineDensity = 0.02f;
            ig.CoinDensity = 0.02f;
            ig.LightningDensity = 0.05f;
            ig.BuildGenDensity();
            gr.IG = ig;

            var lgMgr = srv.Get<LoginManager>();
            lgMgr.OnPlayerLogin += (p) => { gr.AddPlayer(p); };
            lgMgr.OnPlayerLogout += (p) => { gr.RemovePlayer(p); };

            // apis
            GRApis.SendMessageImpl = (id, op, fun) => { SendTo(sc, id, op, fun); };

            GRApis.SC = sc;
        }

        // 登录模块
        static void BuildLoginModule(Server srv, SessionContainer sc, PlayerContainer pc)
        {
            var lgMgr = new LoginManager();
            lgMgr.SC = sc;
            lgMgr.PC = pc;
            srv.Add("LoginMgr", lgMgr);

            var nc = srv.Get<NetCore>();
            nc.OnDisconnected += (conn, r) =>
            {
                var s = sc.GetByConnection(conn);
                if (s == null)
                    return;

                lgMgr.OnLogout(s, r);
            };
        }

        // 碰撞模块
        static void BuildCollisionModule(Server srv, SessionContainer sc)
        {
            Collider.GetPlayerInfo = (id) => sc[id].Player.PlayerInfo;
        }
    }
}
