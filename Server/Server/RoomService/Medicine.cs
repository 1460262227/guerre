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
    /// 医药箱
    /// </summary>
    public class Medicine : MovableObject
    {
        public override void Init()
        {
            base.Init();
            Radius = 0.15f;
            Type = "Medicine";
            CollisionType = "Medicine";
        }
    }
}
