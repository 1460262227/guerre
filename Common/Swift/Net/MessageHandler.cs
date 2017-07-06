using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Swift;

namespace Swift
{
    interface IMessageHandler
    {
        // 处理消息
        void ProcessMessage(Connection conn, IReadableBuffer data, IWriteableBuffer buff, Action end);
    }

    /// <summary>
    /// 具备消息处理能力的对象
    /// </summary>
    public class MessageHandlers : IMessageHandler
    {
        Dictionary<string, Action<Connection, IReadableBuffer>> messageHandlersOnConn = new Dictionary<string, Action<Connection, IReadableBuffer>>();
        Dictionary<string, Action<Connection, IReadableBuffer, IWriteableBuffer>> requestHandlersOnConn = new Dictionary<string, Action<Connection, IReadableBuffer, IWriteableBuffer>>();
        Dictionary<string, Action<Connection, IReadableBuffer, IWriteableBuffer, Action>> requestHandlersOnConnAsync = new Dictionary<string, Action<Connection, IReadableBuffer, IWriteableBuffer, Action>>();

        // 分发消息
        public virtual void ProcessMessage(Connection conn, IReadableBuffer data, IWriteableBuffer buff, Action end)
        {
            var op = data.ReadString();
            HandleOp(conn, op, data, buff, end);
        }

        public virtual void HandleOp(Connection conn, string op, IReadableBuffer data, IWriteableBuffer buff, Action end)
        {
            if (messageHandlersOnConn.ContainsKey(op))
                messageHandlersOnConn[op](conn, data);
            else if (requestHandlersOnConn.ContainsKey(op))
            {
                requestHandlersOnConn[op](conn, data, buff);
                end();
            }
            else if (requestHandlersOnConnAsync.ContainsKey(op))
                requestHandlersOnConnAsync[op](conn, data, buff, end);
        }

        // 注册消息处理方法

        public virtual void OnOp(string op, Action<Connection, IReadableBuffer> cb)
        {
            messageHandlersOnConn[op] = cb;
        }

        public virtual void OnOp(string op, Action<Connection, IReadableBuffer, IWriteableBuffer> cb)
        {
            requestHandlersOnConn[op] = cb;
        }

        public virtual void OnOp(string op, Action<Connection, IReadableBuffer, IWriteableBuffer, Action> cb)
        {
            requestHandlersOnConnAsync[op] = cb;
        }
    }

    /// <summary>
    /// 具备消息处理能力的组件
    /// </summary>
    public class MessageHandlerComponent : NetComponent, IMessageHandler
    {
        protected MessageHandlers mh = null;

        public MessageHandlerComponent(MessageHandlers handler = null)
        {
            if (handler == null)
                handler = new MessageHandlers();

            mh = handler;
        }

        public override void OnMessage(Connection conn, IReadableBuffer data)
        {
            var buff = new WriteBuffer(true);
            ProcessMessage(conn, data, buff, () =>
            {
                if (buff.Available > 0)
                {
                    var rbuff = CreateResponser(conn).BeginResponse();
                    rbuff.Write(buff.Data);
                    conn.End(rbuff);
                }
            });
        }

        // 分发消息
        public virtual void ProcessMessage(Connection conn, IReadableBuffer data, IWriteableBuffer buff, Action end)
        {
            mh.ProcessMessage(conn, data, buff, end);
        }

        // 注册消息处理方法

        public virtual void OnOp(string op, Action<Connection, IReadableBuffer> cb)
        {
            mh.OnOp(op, cb);
        }

        public virtual void OnOp(string op, Action<Connection, IReadableBuffer, IWriteableBuffer> cb)
        {
            mh.OnOp(op, cb);
        }

        public virtual void OnOp(string op, Action<Connection, IReadableBuffer, IWriteableBuffer, Action> cb)
        {
            mh.OnOp(op, cb);
        }
    }
}
