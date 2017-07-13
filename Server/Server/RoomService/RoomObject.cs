using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guerre;
using Swift.Math;

namespace Server
{
    /// <summary>
    /// 服务器端的房间内物体
    /// </summary>
    public abstract class RoomObject : ObjectInfo
    {
        // 所属房间
        public GameRoom Room { get; set; }

        // 可以移除了
        public bool ToBeRemoved { get; set; }

        // 生命值
        public override Fix64 Hp
        {
            get { return base.Hp; }
            set { base.Hp = value; ToBeRemoved = ToBeRemoved || base.Hp <= 0; }
        }
    }
}
