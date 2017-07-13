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
    /// 闪电
    /// </summary>
    public class Lightning : MovableObject
    {
        public override void Init()
        {
            base.Init();
            Radius = 0.15f;
            Type = "Lightning";
            CollisionType = "Lightning";
        }
    }
}
