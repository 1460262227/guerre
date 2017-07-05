using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;

namespace Server
{
    /// <summary>
    /// 玩家信息容器
    /// </summary>
    public class PlayerContainer : DataContainer<Player, string>
    {
        public PlayerContainer(ISqlPersistence<Player, string> p)
            : base(p)
        {

        }
    }
}
