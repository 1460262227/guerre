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
        public static float FrameSec = 0.05f;
        public static int FrameMS = (int)(FrameSec * 1000);

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
            // 加入房间并广播消息
            ops.Add(() =>
            {
                if (p.Room != null)
                    p.Room.RemovePlayer(p);

                p.Room = this;

                // 同步房间信息
                SyncRoomStatus(p.ID);

                var a = new Airplane();
                a.ID = p.ID;
                AddObject(a);
            });
        }

        // 添加物件到房间
        public void AddObject(MovableObject obj)
        {
            if (movableObjs.ContainsKey(obj.ID))
                throw new Exception("object already exists in romm: " + obj.ID + " => " + ID);

            // 加入房间并广播消息
            movableObjs[obj.ID] = obj;
            obj.Room = this;

            Boardcast("AddIn", (buff) =>
            {
                buff.Write(obj.ID);
                buff.Write(obj.Type);
                buff.Write(obj.Pos.x);
                buff.Write(obj.Pos.y);
                buff.Write(obj.Velocity);
                buff.Write(obj.Dir);
                buff.Write(obj.Turn2Dir.x);
                buff.Write(obj.Turn2Dir.y);
                buff.Write(obj.TurnV);
                buff.Write(obj.MaxHp);
                buff.Write(obj.Hp);
                buff.Write(obj.Power);
            });
        }

        // 移除指定物体
        public void RemoveObject(string id)
        {
            if (!movableObjs.ContainsKey(id))
                throw new Exception("object not exists in romm: " + id + " => " + ID);

            var obj = movableObjs[id];
            movableObjs.Remove(id);
            obj.Room = null;

            Boardcast("RemoveOut", (buff) => { buff.Write(id); });
        }

        // 玩家从房间移除
        public void RemovePlayer(Player p)
        {
            ops.Add(() =>
            {
                if (!movableObjs.ContainsKey(p.ID))
                    return;

                RemoveObject(p.ID);
                p.Room = null;
            });
        }

        // 游戏时间流逝
        int timeNumber = 0;
        int timeElapsed = 0;
        public void OnTimeElapsed(int te)
        {
            timeElapsed += te;
            if (timeElapsed < FrameMS)
                return;

            timeElapsed -= FrameMS;

            // 处理这一帧的所有指令
            foreach (var op in ops)
                op();

            ops.Clear();

            // 处理房间内物体逻辑
            ProcessAll(FrameSec);

            // 打印调试信息
            //Console.WriteLine("== t == " + timeNumber);
            //foreach (var obj in movableObjs.Values)
            //    Console.WriteLine("  " + obj.ID + ": (" + obj.Pos.x + ", " + obj.Pos.y + ") : " + obj.Dir);

            // 检查碰撞
            ProcessCollision();

            // 最后做移除操作
            var toBeRemoved = new List<MovableObject>();
            foreach (var k in movableObjs.Keys.ToArray())
            {
                var obj = movableObjs[k];
                if (obj.ToBeRemoved)
                {
                    movableObjs.Remove(k);
                    Boardcast("RemoveOut", (buff) => { buff.Write(k); });
                }
            }

            // 广播游戏时间编号推进
            timeNumber++;
            Boardcast("GameTimeFowardStep");
        }

        // 处理房间内物体逻辑
        void ProcessAll(Fix64 te)
        {
            foreach (var obj in movableObjs.Values)
                obj.OnTimeElapsed(te);
        }

        // 处理碰撞
        void ProcessCollision()
        {
            var needCheck = true;
            var keys = movableObjs.Keys.ToList();

            while (needCheck)
            {
                needCheck = false;
                var cnt = keys.Count;
                FC.For(cnt - 1, (i) =>
                {
                    var k1 = keys[i];
                    var obj1 = movableObjs[k1];
                    FC.For(i + 1, cnt, (j) =>
                    {
                        var k2 = keys[j];
                        var obj2 = movableObjs[k2];
                        if(obj1.DoCollide(obj2))
                        {
                            needCheck = true;
                            keys.Remove(k2);
                            keys.Remove(k1);

                            Boardcast("Collision", (buff) => { buff.Write(k1); buff.Write(k2); });
                        }
                    }, () => needCheck);
                }, () => needCheck);
            }
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
                    var obj = movableObjs[id];
                    buff.Write(obj.ID);
                    buff.Write(obj.Type);
                    buff.Write(obj.Pos.x);
                    buff.Write(obj.Pos.y);
                    buff.Write(obj.Velocity);
                    buff.Write(obj.Dir);
                    buff.Write(obj.Turn2Dir.x);
                    buff.Write(obj.Turn2Dir.y);
                    buff.Write(obj.TurnV);
                    buff.Write(obj.MaxHp);
                    buff.Write(obj.Hp);
                    buff.Write(obj.Power);
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
                var dirTo = new Vec2(data.ReadFix64(), data.ReadFix64());
                var turnV = data.ReadFix64();
                ops.Add(() =>
                {
                    if (!movableObjs.ContainsKey(id))
                        return;

                    var a = movableObjs[id];
                    a.Turn2Dir = dirTo;
                    a.TurnV = turnV;

                    Boardcast("Turn2", (buff) => { buff.Write(id); buff.Write(dirTo.x); buff.Write(dirTo.y); buff.Write(turnV); });
                });
            });

            OnOp("UseSkill", (Session s, IReadableBuffer data) =>
            {
                var id = s.ID;
                var skillName = data.ReadString();
                ops.Add(() =>
                {
                    if (!movableObjs.ContainsKey(id))
                        return;

                    var a = movableObjs[id] as Airplane;
                    a.UseSkill(skillName);
                });
            });
        }
    }
}
