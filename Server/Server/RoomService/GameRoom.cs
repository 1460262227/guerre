﻿using System;
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
    public class GameRoom : MessageHandlerComponent
    {
        Dictionary<string, Airplane> airplanes = new Dictionary<string, Airplane>();

        // 房间 ID
        public string ID { get; private set; }

        public GameRoom(string id)
        {
            ID = id;
        }

        // 初始化刚进入房间的玩家的飞机信息
        void InitAirplane(Airplane a)
        {
            a.Pos = Vec2.Zero;
            a.Dir = MathEx.HalfPi;
            a.Velocity = 1;
        }

        // 添加玩家到房间
        public void AddPlayer(Player p)
        {
            if (airplanes.ContainsKey(p.ID))
                throw new Exception("player already exists in romm: " + p.ID + " => " + ID);

            if (p.Room != null)
                p.Room.RemovePlayer(p);

            p.Room = this;

            var a = new Airplane();
            InitAirplane(a);
            airplanes[p.ID] = a;

            Boardcast("AddIn", (buff) => { buff.Write(p.ID); });
        }

        // 玩家从房间移除
        public void RemovePlayer(Player p)
        {
            if (!airplanes.ContainsKey(p.ID))
                throw new Exception("player not exists in romm: " + p.ID + " => " + ID);

            airplanes.Remove(p.ID);
            p.Room = null;

            Boardcast("RemoveOut", (buff) => { buff.Write(p.ID); });
        }

        // 游戏时间流逝
        int timeNumber = 0;
        int timeElapsed = 0;
        public void OnTimeElapsed(int te)
        {
            timeElapsed += te;
            if (timeElapsed < 100)
                return;

            // 广播游戏时间编号推进
            timeNumber++;
            Boardcast("GameTimeFowardStep");
        }

        // 房间内广播消息
        void Boardcast(string op, Action<IWriteableBuffer> fun = null)
        {
            foreach (var id in airplanes.Keys)
                GRApis.SendMessage(id, op, fun);
        }
    }
}
