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
        List<Player> players = new List<Player>();

        // 击杀统计
        Dictionary<string, int> kills = new Dictionary<string, int>();

        // 房间 ID
        public string ID { get; private set; }

        // 物品随机生成
        public ItemGenerator IG = null;

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
                players.Add(p);

                if (!kills.ContainsKey(p.ID))
                    kills[p.ID] = 0;

                // 同步房间信息
                SyncRoomStatus(p.ID);
            });
        }

        // 添加物件到房间
        public void AddObject(MovableObject obj)
        {
            if (movableObjs.ContainsKey(obj.ID))
                throw new Exception("object already exists in romm: " + obj.ID + " => " + ID);

            var id = obj.ID;

            // 加入房间并广播消息
            movableObjs[id] = obj;
            obj.Room = this;

            Boardcast("AddIn", (buff) =>
            {
                obj.Serialize(buff);
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
                players.Remove(p);
                p.Room = null;

                if (movableObjs.ContainsKey(p.ID))
                    RemoveObject(p.ID);
            });
        }

        // 临时做个安全区检查，范围外扣血
        public Vec2 SafeAreaLeftTop;
        public Vec2 SafeAreaSize;
        bool InSafeArea(Vec2 pos)
        {
            var p = pos - SafeAreaLeftTop;
            return p.x > 0 && p.x < SafeAreaSize.x && p.y > 0 && p.y < SafeAreaSize.y;
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

            // 物品生成
            if (IG != null)
            {
                IG.RefreshItemStatistics();
                IG.RandomGen(FrameSec, (obj) => { AddObject(obj); });
            }

            // 检查碰撞
            ProcessCollision();

            // 处理房间内物体逻辑
            ProcessAll(FrameSec);

            // 临时安全区检查
            foreach (var obj in movableObjs.Values)
            {
                if (!InSafeArea(obj.Pos))
                    obj.Hp -= FrameSec / 2;
            }

            // 最后做移除操作`
            var toBeRemoved = new List<MovableObjectInfo>();
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
            // 打印调试信息
            //Console.WriteLine("== t == " + timeNumber);

            foreach (var obj in movableObjs.Values)
            {
                obj.ProcessLogic(te);

                //if (obj.CollisionType == "Airplane")
                //    Console.WriteLine("v = " + obj.Velocity);

                obj.ProcessMove(te);

                // if (obj.CollisionType == "Airplane")
                //  Console.WriteLine(" " + obj.ID + ": (" + obj.Pos.x + ", " + obj.Pos.y + ") : " + obj.Dir);
            }
        }

        // 刷新击杀统计
        void RefreshKillStatistics(string killer, string target)
        {
            var t = movableObjs[target] as Airplane;
            if (t == null || t.Hp > 0)
                return;

            if (movableObjs[killer] is SmallBullet)
                killer = (movableObjs[killer] as SmallBullet).OwnerID;

            kills[killer]++;

            Boardcast("Killing", (buff) => { buff.Write(killer); });
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
                        if (Collider.DoCollision(obj1, obj2))
                        {
                            needCheck = true;
                            keys.Remove(k2);
                            keys.Remove(k1);

                            RefreshKillStatistics(k1, k2);
                            RefreshKillStatistics(k2, k1);

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
                buff.Write(SafeAreaLeftTop.x);
                buff.Write(SafeAreaLeftTop.y);
                buff.Write(SafeAreaSize.x);
                buff.Write(SafeAreaSize.y);
                buff.Write(timeNumber);

                // 房间内物体信息
                buff.Write(movableObjs.Count);
                foreach (var id in movableObjs.Keys)
                {
                    var obj = movableObjs[id];
                    obj.Serialize(buff);
                }

                // 击杀信息
                buff.Write(kills.Count);
                foreach (var k in kills.Keys)
                {
                    buff.Write(k);
                    buff.Write(kills[k]);
                }
            });
        }

        // 房间内广播消息
        public void Boardcast(string op, Action<IWriteableBuffer> fun = null)
        {
            foreach (var p in players)
                GRApis.SendMessage(p.ID, op, (buff) => { buff.Write(timeNumber); fun.SC(buff); } );
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

            OnOp("Join", (Session s, IReadableBuffer data) =>
            {
                var id = s.ID;
                var lv = data.ReadInt();
                ops.Add(() =>
                {
                    var a = new Airplane();
                    a.ID = id;
                    a.BuildAttrs(lv);
                    AddObject(a);
                });
            });
        }
    }
}
