using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;

namespace Server
{
    /// <summary>
    /// 用户会话
    /// </summary>
    public class Session
    {
        // 就是用户 ID
        public string ID { get; private set; }

        public Session(string id)
        {
            ID = id;
        }

        // 当前网络链接对象
        public Connection Connection { get; set; }

        // 当前玩家对象
        public Player Player { get; set; }
    }
}
