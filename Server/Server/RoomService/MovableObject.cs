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
    /// 房间内的可移动物体
    /// </summary>
    public abstract class MovableObject
    {
        // 唯一 ID
        public string ID;

        // 物体类型
        public abstract string Type { get; }

        // 移动速率
        public float Velocity { get; set; }

        // 角速度
        public float TurnV { get; set; }

        // 当前位置
        public Vec2 Pos { get; set; }

        // 当前方向(沿 x 正方向顺时针，弧度)
        public float Dir { get; set; }

        // 转向目标方向
        public Vec2 Turn2Dir { get; set; }


        // 当前方向的 Vector2 表达
        public Vec2 DirV2
        {
            get
            {
                var dir = Dir;
                return new Vec2((float)Math.Cos(dir), (float)Math.Sin(dir));
            }
            set
            {
                Dir = value.Dir();
            }
        }

        // 沿当前方向移动一段距离
        public void MoveForward(float te)
        {
            // 更新角度
            if (TurnV != 0)
            {
                var da = MathEx.CalcDir4Turn2(DirV2, Turn2Dir, TurnV);
                Dir += da;
            }

            var d = te * Velocity;
            var dx = (float)Math.Cos(Dir) * d;
            var dy = (float)Math.Sin(Dir) * d;
            Pos += new Vec2(dx, dy);
        }
    }
}
