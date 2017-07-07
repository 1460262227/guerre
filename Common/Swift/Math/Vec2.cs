using System;
using System.Collections.Generic;
using System.Text;

namespace Swift.Math
{
    // 2D 浮点向量
    public struct Vec2
    {
        public Fix64 x;
        public Fix64 y;

        public Vec2(float vx, float vy)
        {
            x = vx;
            y = vy;
        }

        public Vec2(Fix64 vx, Fix64 vy)
        {
            x = vx;
            y = vy;
        }

        public Fix64 Length
        {
            get
            {
                return Fix64.Sqrt(x * x + y * y);
            }
        }

        public static Vec2 operator + (Vec2 v1, Vec2 v2)
        {
            return new Vec2(v1.x + v2.x, v1.y + v2.y);
        }

        public static Vec2 operator - (Vec2 v1, Vec2 v2)
        {
            return new Vec2(v1.x - v2.x, v1.y - v2.y);
        }

        public static Vec2 operator *(Vec2 v, Fix64 scale)
        {
            return new Vec2(v.x * scale, v.y * scale);
        }

        public static Vec2 operator *(Fix64 scale, Vec2 v)
        {
            return new Vec2(v.x * scale, v.y * scale);
        }

        public static Vec2 operator /(Vec2 v, Fix64 scale)
        {
            return new Vec2(v.x / scale, v.y / scale);
        }

        public static Vec2 operator -(Vec2 v)
        {
            return new Vec2(-v.x, -v.y);
        }

        public static bool operator == (Vec2 v1, Vec2 v2)
        {
            return v1.x == v2.x && v1.y == v2.y;
        }

        public static bool operator !=(Vec2 v1, Vec2 v2)
        {
            return !(v1 == v2);
        }

        public override bool Equals(object obj)
        {
            return this == (Vec2)obj;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public void Normalize()
        {
            var len = Length;
            if (len != 0)
            {
                x /= len;
                y /= len;
            }
            else
            {
                x = 1;
                y = 0;
            }
        }

        // 求垂线(Left 方向)
        public Vec2 PerpendicularL
        {
            get
            {
                return new Vec2(-y, x);
            }
        }

        public static Vec2 Zero = new Vec2(0, 0);
        public static Vec2 Left = new Vec2(-1, 0);
        public static Vec2 Right = new Vec2(1, 0);
        public static Vec2 Up = new Vec2(0, 1);
        public static Vec2 Down = new Vec2(0, -1);
    }
}
