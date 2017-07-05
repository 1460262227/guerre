using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;

namespace Server
{
    /// <summary>
    /// 游戏房间模块所依赖功能
    /// </summary>
    public static class GRApis
    {
        public static string OpGroup = "GameRoom";
        public static SessionContainer SC = null;

        // 发送网络消息
        public static Action<string, string, Action<IWriteableBuffer>> SendMessageImpl = null;
        public static void SendMessage(string idTo, string op, Action<IWriteableBuffer> fun)
        {
            SendMessageImpl(idTo, OpGroup + "/" + op, fun);
        }
    }
}
