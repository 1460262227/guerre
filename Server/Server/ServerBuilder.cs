using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Swift;
using Server;

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
            ServerLogger log = new ServerLogger(typeof(ServerBuilder));

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(HandleAllUnhandledException);

            // 网络模块
            NetCore nc = new NetCore();
            srv.Add("NetCore", nc);

            // 所有初始化完成后，启动服务器的网络监听
            log.Info("server started at port: " + port);
            nc.StartListening(ip, port);
        }

        // 处理所有未被处理的异常信息
        static void HandleAllUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ServerLogger log = new ServerLogger(typeof(UnhandledExceptionEventArgs));
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
    }
}
