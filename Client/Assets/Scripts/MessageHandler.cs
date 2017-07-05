using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Swift;

/// <summary>
/// 统一处理所有服务器消息
/// </summary>
public class MessageHandler : MessageHandlerComponent
{
    GameCore GC { get { return GameCore.Instance; } }
    Connection SrvConn { get { return GameCore.Instance.CurrentServerConnection; } }

    // 发送消息给服务器
    public void Send(string com, string op, Action<IWriteableBuffer> fun)
    {
        var buff = SrvConn.BeginSend(com);
        buff.Write(op);
        if (fun != null)
            fun(buff);
        SrvConn.End(buff);
    }

    // 发送请求给服务器
    public void Request(string com, string op, Action<IWriteableBuffer> fun, Action<IReadableBuffer> cb, Action<bool> onExpired = null)
    {
        var buff = SrvConn.BeginRequest(com, cb, onExpired);
        buff.Write(op);
        if (fun != null)
            fun(buff);
        SrvConn.End(buff);
    }
}
