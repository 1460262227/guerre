using System;
using System.Collections.Generic;
using System.Threading;
using Swift;

namespace Server
{
    /// <summary>
    /// 服务器对象
    /// </summary>
    public class Server : Core
    {
        ServerLogger log = new ServerLogger(typeof(Server));

        // 服务器逻辑真间隔（毫秒）
        public int Interval = 10;

        public Server()
        {
            // 加入默认功能组件
            Add("CoroutineManager", new CoroutineManager());
        }

        public void Start()
        {
            running = true;
            long t = Utils.Now;

            log.Info("server starting ...");

            while (running)
            {
                long now = Utils.Now;
                int dt = (int)(now - t);

                try
                {
                    RunOneFrame(dt);
                }
                catch (Exception ex)
                {
                    log.Error("\r\n==========\r\n" + ex.Message + "\r\n==========\r\n" + ex.StackTrace + "\r\n==========\r\n");
                }

                t = now;

                // sleep according to interval
                int sleepTime = Interval > dt ? Interval - dt : 0;
                Thread.Sleep(sleepTime);
            }

            log.Info("server stopped.");
        }

        // 停止服务器
        public void Stop()
        {
            Close();
            running = false;
        }

        #region 保护部分

        // 运行中标记
        bool running = false;

        #endregion
    }
}