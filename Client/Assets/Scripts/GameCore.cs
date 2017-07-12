using System.Collections.Generic;
using Swift;
using Guerre;
using System;

/// <summary>
/// 客户端游戏核心对象，引用所有逻辑模块并负责主要驱动
/// </summary>
public class GameCore : Core
{
    static GameCore instance = null;
    public static GameCore Instance
    {
        get
        {
            if (instance == null)
                instance = new GameCore();
            return instance;
        }
    }

    public void Init()
    {
        // 网络核心
        var nc = new NetCore();
        Add("NetCore", nc);

        // 消息处理模块
        var mm = new MessageHandler();
        Add("MessageHandler", mm);
        APIs.Send = mm.Send;
        APIs.Request = mm.Request;
    }

    // 连接服务器
    public void ConnectServer(string ip, int port, System.Action<Connection, string> callback)
    {
        NetCore nc = Get<NetCore>();
        nc.Close();

        UnityEngine.Debug.Log("ConnectServer " + ip + ":" + port);

        nc.Connect2Peer(ip, port, (Connection conn, string reason) =>
        {
            srvConn = conn;
            callback(conn, reason);
        });
    }

    // 关闭网络连接
    public void CloseNetConnections()
    {
        NetCore nc = Get<NetCore>();
        nc.Close();
    }

    // 当前服务器连接对象
    public Connection CurrentServerConnection { get { return srvConn; } }
    Connection srvConn = null;

    public void OnTimeElapsed(int te)
    {
        RunOneFrame(te);
    }

    public PlayerInfo Me = null;
    public MovableObjectController MeOC = null;
    public int CurSelAirplane = 0;

    // 加入房间和移出房间
    public Action RoomSynchonizing = null;
    public Action OnIn = null;
    public Action OnOut = null;

    // 击杀统计
    public Action<string, int> OnKill = null;
    public int MyKills = 0; // 击杀统计
}
