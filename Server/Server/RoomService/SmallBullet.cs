using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;
using Swift.Math;

namespace Server
{
    /// <summary>
    /// 小子弹
    /// </summary>
    public class SmallBullet : MovableObject
    {
        public override string Type => "Bullet";

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
        }

        // 沿当前方向移动一段距离
        public override void OnTimeElapsed(Fix64 te)
        {
            var d = MoveForward(te);
            RangeLeft -= d;
            ToBeRemoved = ToBeRemoved || RangeLeft <= 0;
        }

        // 碰撞
        public override bool CheckCollide(MovableObject obj)
        {
            var d2 = (Pos - obj.Pos).Length2;
            return d2 < Radius2 + obj.Radius2;
        }
    }
}
