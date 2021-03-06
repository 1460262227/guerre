﻿using System;
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

        // 玩家登入/登出事件
        public event Action<Player> OnPlayerLogin = null;
        public event Action<Player> OnPlayerLogout = null;

        public LoginManager()
        {
            OnOp("login", OnLogin);
            OnOp("unlock", OnUnlock);
        }

        // 升级解锁
        public void OnUnlock(Session s, IReadableBuffer data, IWriteableBuffer buff)
        {
            var lv = data.ReadInt();
            var cost = lv * 10;

            var p = s.Player;
            var pi = p.PlayerInfo;
            if (pi.Money >= cost)
            {
                pi.Money -= cost;
                pi.Level = lv;
                buff.Write(true);
                buff.Write(pi.Money);
                buff.Write(pi.Level);
            }
            else
                buff.Write(false);
        }

        // 玩家登录
        public void OnLogin(Connection conn, IReadableBuffer data, IWriteableBuffer buff, Action end)
        {
            var id = data.ReadString();
            var pwd = data.ReadString();

            // 重复登录就先登出
            if (SC.ContainsKey(id))
                OnLogout(SC[id], "relogin");

            var s = new Session(id);
            s.Connection = conn;
            OnLogin(s, id, pwd, (p, r) =>
            {
                buff.Write(r);
                if (p != null)
                    p.PlayerInfo.Serialize(buff); end();
            });
        }

        // 玩家登录
        public void OnLogin(Session s, string id, string pwd, Action<Player, bool> r)
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
                    r(p, true);

                    OnPlayerLogin.SC(p);
                }
                else
                    r(null, false);
            });
        }

        // 玩家登出
        public void OnLogout(Session s, string reason)
        {
            SC.Remove(s.ID);
            s.Connection.Close();
            OnPlayerLogout.SC(s.Player);
        }
    }
}
