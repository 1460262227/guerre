using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;
using Guerre;

namespace Server
{
    /// <summary>
    /// 玩家对象
    /// </summary>
    public class Player : UniqueDataItem
    {
        // 玩家信息
        public PlayerInfo PlayerInfo
        {
            get { return playerInfo; }
            set { playerInfo = value; ID = playerInfo.ID; }
        } PlayerInfo playerInfo;

        // 所在房间
        public GameRoom Room { get; set; }

        protected override void Sync()
        {
            base.Sync();
            BeginSync();
            {
                SyncObj(ref playerInfo);
            }
            EndSync();
        }
    }
}
