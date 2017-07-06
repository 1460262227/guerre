using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guerre;
using Swift;
using Swift.Math;

namespace Server
{
    /// <summary>
    /// 一个游戏房间
    /// </summary>
    public class GameRoom : ServerMessageHandlerComponent
    {
        Dictionary<string, MovableObject> movableObjs = new Dictionary<string, MovableObject>();

        // 房间 ID
        public string ID { get; private set; }

        public GameRoom(string id)
        {
            ID = id;

            RegisterAllMessages();
        }

        // 添加玩家到房间
        public void AddPlayer(Player p)
        {
            ops.Add(() =>
            {
                if (movableObjs.ContainsKey(p.ID))
                    throw new Exception("player already exists in romm: " + p.ID + " => " + ID);

                if (p.Room != null)
                    p.Room.RemovePlayer(p);

                // 先同步房间信息
                SyncRoomStatus(p.ID);

                // 加入房间并广播消息
                var a = new Airplane();
                a.Init();
                movableObjs[p.ID] = a;
                p.Room = this;

                Boardcast("AddIn", (buff) => 
                {
                    buff.Write(p.ID);
                    buff.Write(a.Type);
                    buff.Write(a.Pos.x);
                    buff.Write(a.Pos.y);
                    buff.Write(a.Velocity);
                    buff.Write(a.Dir);
                    buff.Write(a.Turn2Dir.x);
                    buff.Write(a.Turn2Dir.y);
                    buff.Write(a.TurnV);
                });
            });
        }

        // 玩家从房间移除
        public void RemovePlayer(Player p)
        {
            ops.Add(() =>
            {
                if (!movableObjs.ContainsKey(p.ID))
                    throw new Exception("player not exists in romm: " + p.ID + " => " + ID);

                movableObjs.Remove(p.ID);
                p.Room = null;

                Boardcast("RemoveOut", (buff) => { buff.Write(p.ID); });
            });
        }

        // 游戏时间流逝
        int timeNumber = 0;
        int timeElapsed = 0;
        public void OnTimeElapsed(int te)
        {
            timeElapsed += te;
            if (timeElapsed < 100)
                return;

            timeElapsed -= 100;

            // 处理这一帧的所有指令
            foreach (var op in ops)
                op();

            ops.Clear();

            // 广播游戏时间编号推进
            Boardcast("GameTimeFowardStep");
            timeNumber++;

            // 房间内所有物体移动 100 毫秒
            MoveAll(0.1f);
        }

        void MoveAll(float te)
        {
            foreach (var obj in movableObjs.Values)
                obj.MoveForward(te);
        }

        // 当前帧需要执行的指令
        List<Action> ops = new List<Action>();

        // 同步房间状态
        void SyncRoomStatus(string idTo)
        {
            GRApis.SendMessage(idTo, "SyncRoom", (buff) =>
            {
                buff.Write(timeNumber);
                buff.Write(movableObjs.Count);
                foreach (var id in movableObjs.Keys)
                {
                    var a = movableObjs[id];
                    buff.Write(id);
                    buff.Write(a.Type);
                    buff.Write(a.Pos.x);
                    buff.Write(a.Pos.y);
                    buff.Write(a.Velocity);
                    buff.Write(a.Dir);
                    buff.Write(a.Turn2Dir.x);
                    buff.Write(a.Turn2Dir.y);
                    buff.Write(a.TurnV);
                }
            });
        }

        // 房间内广播消息
        void Boardcast(string op, Action<IWriteableBuffer> fun = null)
        {
            foreach (var id in movableObjs.Keys)
                GRApis.SendMessage(id, op, (buff) => { buff.Write(timeNumber); fun.SC(buff); } );
        }

        // 注册所有消息处理函数
        void RegisterAllMessages()
        {
            OnOp("Turn2", (Session s, IReadableBuffer data) =>
            {
                var id = s.ID;
                var dirTo = new Vec2(data.ReadFloat(), data.ReadFloat());
                var turnV = data.ReadFloat();
                ops.Add(() =>
                {
                    if (!movableObjs.ContainsKey(id))
                        throw new Exception("player not exists in romm: " + id + " => " + ID);

                    var a = movableObjs[id];
                    a.Turn2Dir = dirTo;
                    a.TurnV = turnV;

                    Boardcast("Turn2", (buff) => { buff.Write(id); buff.Write(dirTo.x); buff.Write(dirTo.y); buff.Write(turnV); });
                });
            });
        }
    }
}
