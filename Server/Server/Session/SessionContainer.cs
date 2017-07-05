using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;

namespace Server
{
    /// <summary>
    /// 用户会话容器
    /// </summary>
    public class SessionContainer : Dictionary<string, Session>
    {
        // 根据网络链接获取会话对象
        public Session GetByConnection(Connection conn)
        {
            foreach (var s in Values)
            {
                if (s.Connection == conn)
                    return s;
            }

            return null;
        }

        // 根据 ID 获取会话对象
        public Session Get(string id)
        {
            return ContainsKey(id) ? this[id] : null;
        }
    }
}
