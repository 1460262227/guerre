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
    /// 医药箱
    /// </summary>
    public class Medicine : MovableObject
    {
        public override string Type => "Medicine";

        public override void Init()
        {
            base.Init();

            Power = -1;
            Radius = 0.15f;
        }

        // 碰撞
        public override bool CheckCollide(MovableObject obj)
        {
            if (!(obj is Airplane))
                return false;

            var d2 = (Pos - obj.Pos).Length2;
            return d2 < Radius2 + obj.Radius2;
        }
    }
}
