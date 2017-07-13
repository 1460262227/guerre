using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;
using Swift.Math;
using Guerre;

namespace Server
{
    /// <summary>
    /// 金币
    /// </summary>
    public class Coin : MovableObject
    {
        public override void Init()
        {
            base.Init();
            Radius = 0.15f;
            Type = "Coin";
            CollisionType = "Coin";
        }
    }
}
