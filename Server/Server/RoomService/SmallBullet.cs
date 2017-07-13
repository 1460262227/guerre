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
    /// 小子弹
    /// </summary>
    public class SmallBullet : MovableObject
    {
        // 谁发的子弹
        public string OwnerID = null;

        // 剩余的射程
        public Fix64 RangeLeft = 0;

        public override void Init()
        {
            base.Init();

            Velocity = 1;
            Radius = 0.1f;
            Power = 1;
            MaxHp = Hp = 1;
            RangeLeft = 1;
            Type = "SmallBullet";
            CollisionType = "Bullet";
        }

        public override void ProcessMove(Fix64 te)
        {
            var d = MoveForward(te);
            RangeLeft -= d;
            ToBeRemoved = ToBeRemoved || RangeLeft <= 0;
        }
    }
}
