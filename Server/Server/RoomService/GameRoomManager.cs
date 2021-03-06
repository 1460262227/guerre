﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;

namespace Server
{
    /// <summary>
    /// 管理游戏房间逻辑
    /// </summary>
    public class GameRoomManager : ServerMessageHandlerComponent, IFrameDrived
    {
        public GameRoomContainer GRC = null;

        // 创建新房间
        public GameRoom CreateNewRoom(string id)
        {
            if (GRC.ContainsKey(id))
                throw new Exception("Game room id conflict: " + id);

            var gr = new GameRoom(id);
            gr.SC = SC;
            GRC[id] = gr;
            AddComponent("GameRoom/" + id, gr);
            return gr;
        }

        public void OnTimeElapsed(int te)
        {
            GRC.OnTimeElapsed(te);
        }
    }
}
