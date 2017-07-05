using System;

namespace Swift.Math
{
    // 零碎扩展
    public static class MathEx
    {
        public const float Pi = (float) System.Math.PI;
        public const float HalfPi = Pi / 2;
        public const float Pi2 = Pi * 2;

        public const float Left = Pi;
        public const float Right = 0;
        public const float Up = HalfPi;
        public const float Down = HalfPi + Pi;

        // 向量所指方向对应的欧拉角(0-2pi)
        public static float Dir(this Vec2 v)
        {
            // 正则化
            var len = v.Length;
            var x = v.x / len;
            var y = v.y / len;

            if (System.Math.Abs(x) <= float.Epsilon)
                return y > 0 ? HalfPi : -HalfPi;
            else
            {
                var tanValue = y / x;
                var arc = (float)System.Math.Atan(tanValue);

                if (x < 0)
                    arc += Pi;

                if (arc < 0)
                    arc += Pi2;

                return arc;
            }
        }
        
        // 截断到给定范围
        public static float Clamp(this float v, float min, float max)
        {
            if (v < min)
                return min;
            else if (v > max)
                return max;
            else
                return v;
        }
    }
}
