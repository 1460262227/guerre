using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    /// <summary>
    /// 子弹
    /// </summary>
    public class Bullet : MovableObject
    {
        public override string Type => "Bullet";

        // 剩余的射程
        public float RangeLeft = 0;

        public override void Init()
        {
            base.Init();
            RangeLeft = 2;
        }

        // 沿当前方向移动一段距离
        public override void OnTimeElapsed(float te)
        {
            var d = MoveForward(te);
            RangeLeft -= d;
            ToBeRemoved = RangeLeft <= 0;
        }
    }
}
