using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;
using Guerre;

namespace Server
{
    /// <summary>
    /// 登录管理器
    /// </summary>
    public class LoginManager : ServerMessageHandlerComponent
    {
        public PlayerContainer PC = null;

        public LoginManager()
        {
            OnOp("login", OnLogin);
            OnOp("login", (Session s, IReadableBuffer data, IWriteableBuffer buff, Action end) =>
            {
                var id = data.ReadString();
                var pwd = data.ReadString();
                OnLogin(s, id, pwd, (r) => { buff.Write(r); end(); });
            });
        }

        // 玩家登录
        public void OnLogin(Connection conn, IReadableBuffer data, IWriteableBuffer buff, Action end)
        {
            var id = data.ReadString();
            var pwd = data.ReadString();

            var s = new Session(id);
            s.Connection = conn;
            OnLogin(s, id, pwd, (r) => { buff.Write(r); end(); });
        }

        // 玩家登录
        public void OnLogin(Session s, string id, string pwd, Action<bool> r)
        {
            PC.Retrieve(id, (p) =>
            {
                if (p == null)
                {
                    var pi = new PlayerInfo();
                    pi.ID = id;
                    pi.Name = id;
                    pi.PWD = pwd;
                    p = new Player();
                    p.PlayerInfo = pi;
                    PC.AddNew(p);
                }

                if (p.PlayerInfo.PWD == pwd)
                {
                    s.Player = p;
                    SC[id] = s;
                    r(true);
                }
                else
                    r(false);
            });
        }
    }
}
