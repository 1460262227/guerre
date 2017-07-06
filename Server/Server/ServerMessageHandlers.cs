using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;

namespace Server
{
    /// <summary>
    /// 具备消息处理能力的对象
    /// </summary>
    public class ServerMessageHandlers : MessageHandlers
    {
        public SessionContainer SC = null;

        Dictionary<string, Action<Connection, IReadableBuffer>> messageHandlersOnConn = new Dictionary<string, Action<Connection, IReadableBuffer>>();
        Dictionary<string, Action<Connection, IReadableBuffer, IWriteableBuffer>> requestHandlersOnConn = new Dictionary<string, Action<Connection, IReadableBuffer, IWriteableBuffer>>();
        Dictionary<string, Action<Connection, IReadableBuffer, IWriteableBuffer, Action>> requestHandlersOnConnAsync = new Dictionary<string, Action<Connection, IReadableBuffer, IWriteableBuffer, Action>>();

        Dictionary<string, Action<Session, IReadableBuffer>> messageHandlersOnSesson = new Dictionary<string, Action<Session, IReadableBuffer>>();
        Dictionary<string, Action<Session, IReadableBuffer, IWriteableBuffer>> requestHandlersOnSesson = new Dictionary<string, Action<Session, IReadableBuffer, IWriteableBuffer>>();
        Dictionary<string, Action<Session, IReadableBuffer, IWriteableBuffer, Action>> requestHandlersOnSessionAsync = new Dictionary<string, Action<Session, IReadableBuffer, IWriteableBuffer, Action>>();

        // 分发消息
        public override void ProcessMessage(Connection conn, IReadableBuffer data, IWriteableBuffer buff, Action end)
        {
            var op = data.ReadString();
            var s = SC.GetByConnection(conn);

            if (s == null || !HandleOp(s, op, data, buff, end))
                HandleOp(conn, op, data, buff, end);
        }

        public virtual bool HandleOp(Session s, string op, IReadableBuffer data, IWriteableBuffer buff, Action end)
        {
            if (messageHandlersOnSesson.ContainsKey(op))
            {
                messageHandlersOnSesson[op](s, data);
                return true;
            }
            else if (requestHandlersOnSesson.ContainsKey(op))
            {
                requestHandlersOnSesson[op](s, data, buff);
                end();
                return true;
            }
            else if (requestHandlersOnConnAsync.ContainsKey(op))
            {
                requestHandlersOnSessionAsync[op](s, data, buff, end);
                return true;
            }

            return false;
        }

        // 注册消息处理方法

        public virtual void OnOp(string op, Action<Session, IReadableBuffer> cb)
        {
            messageHandlersOnSesson[op] = cb;
        }

        public virtual void OnOp(string op, Action<Session, IReadableBuffer, IWriteableBuffer> cb)
        {
            requestHandlersOnSesson[op] = cb;
        }

        public virtual void OnOp(string op, Action<Session, IReadableBuffer, IWriteableBuffer, Action> cb)
        {
            requestHandlersOnSessionAsync[op] = cb;
        }
    }

    /// <summary>
    /// 具备消息处理能力的组件
    /// </summary>
    public class ServerMessageHandlerComponent : MessageHandlerComponent, IMessageHandler
    {
        ServerMessageHandlers smh
        {
            get { return mh as ServerMessageHandlers; }
        }

        public SessionContainer SC
        {
            get { return smh.SC; }
            set { smh.SC = value; }
        }

        public ServerMessageHandlerComponent() : base (new ServerMessageHandlers()) { }

        // 注册消息处理方法

        public virtual void OnOp(string op, Action<Session, IReadableBuffer> cb)
        {
            smh.OnOp(op, cb);
        }

        public virtual void OnOp(string op, Action<Session, IReadableBuffer, IWriteableBuffer> cb)
        {
            smh.OnOp(op, cb);
        }

        public virtual void OnOp(string op, Action<Session, IReadableBuffer, IWriteableBuffer, Action> cb)
        {
            smh.OnOp(op, cb);
        }
    }
}
