using System;
using System.Collections.Generic;
using System.Text;

namespace Swift.Math
{
    // 2D 浮点向量
    public struct Vec2
    {
        public float x;
        public float y;

        public Vec2(float vx, float vy)
        {
            x = vx;
            y = vy;
        }

        public float Length
        {
            get
            {
                return (float)System.Math.Sqrt(x * x + y * y);
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

        public static Vec2 operator *(Vec2 v, float scale)
        {
            return new Vec2(v.x * scale, v.y * scale);
        }

        public static Vec2 operator *(float scale, Vec2 v)
        {
            return new Vec2(v.x * scale, v.y * scale);
        }

        public static Vec2 operator /(Vec2 v, float scale)
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
            if (System.Math.Abs(len) > float.Epsilon)
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

        public static Vec2 Zero = new Vec2(0, 0);
        public static Vec2 Left = new Vec2(-1, 0);
        public static Vec2 Right = new Vec2(1, 0);
        public static Vec2 Up = new Vec2(0, 1);
        public static Vec2 Down = new Vec2(0, -1);
    }
}
