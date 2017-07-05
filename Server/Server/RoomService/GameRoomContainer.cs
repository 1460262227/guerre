using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;

namespace Server
{
    /// <summary>
    /// 存放所有游戏房间
    /// </summary>
    public class GameRoomContainer : Dictionary<string, GameRoom>, IFrameDrived
    {
        public void OnTimeElapsed(int te)
        {
            foreach (var room in Values)
                room.OnTimeElapsed(te);
        }
    }
}
