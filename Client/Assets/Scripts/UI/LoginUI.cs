using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : UIBase {

    public Text Acc;
    public Text Pwd;
    public Text Tips;

    // 执行登录操作
    public void OnLogin()
    {
        var ip = "127.0.0.1";
        var port = 9600;
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

            APIs.RequestImpl("LoginMgr", "login", (buff) =>
            {
                buff.Write(acc);
                buff.Write(pwd);
            }, (data) =>
            {
                var ok = data.ReadBool();
                if (!ok)
                {
                    Tips.text = "登录失败";
                    conn.Close();
                }
                else
                    Hide();
            }, (connected) =>
            {
                Tips.text = "登录超时";
                if (connected)
                    conn.Close();
            });
        });
    }
}
