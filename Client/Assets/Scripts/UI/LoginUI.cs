using Guerre;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : UIBase {

    public Text IP;
    public Text Acc;
    public Text Pwd;
    public Text Tips;

    public UIBase RebornUI;
    public UIBase SelAirplaneUI;

    private void Start()
    {
        RebornUI.Hide();
        SelAirplaneUI.Hide();
    }

    // 执行登录操作
    public void OnLogin()
    {
        var ipport = IP.text.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        var ip = ipport[0];
        var port = int.Parse(ipport[1]);
        var acc = Acc.text;
        var pwd = Pwd.text;

        var gc = GameCore.Instance;
        Tips.text = "正在连接服务器 => " + ip + ":" + port + " ...";
        gc.ConnectServer(ip, port, (conn, r) =>
        {
            if (conn == null)
            {
                Tips.text = "连接失败: " + r;
                return;
            }

            APIs.Request("LoginMgr", "login", (buff) =>
            {
                buff.Write(acc);
                buff.Write(pwd);
            }, (data) =>
            {
                var ok = data.ReadBool();
                if (!ok)
                {
                    Tips.text = "登录失败";
                    GameCore.Instance.Me = null;
                    conn.Close();
                }
                else
                {
                    Hide();
                    SelAirplaneUI.Show();
                    var pi = new PlayerInfo();
                    pi.Deserialize(data);
                    GameCore.Instance.Me = pi;
                }
            }, (connected) =>
            {
                Tips.text = "登录超时";
                if (connected)
                    conn.Close();
            });
        });
    }
}
