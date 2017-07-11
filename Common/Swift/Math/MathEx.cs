using System;

namespace Swift.Math
{
    // 零碎扩展
    public static class MathEx
    {
        public static readonly Fix64 Pi = Fix64.Pi;
        public static readonly Fix64 HalfPi = Pi / 2;
        public static readonly Fix64 Pi2 = Pi * 2;

        public static readonly Fix64 Left = Fix64.Pi;
        public static readonly Fix64 Right = Fix64.Zero;
        public static readonly Fix64 Up = HalfPi;
        public static readonly Fix64 Down = HalfPi + Pi;

        public static readonly Fix64 Rad2Deg = 180 / Pi;
        public static readonly Fix64 Deg2Rad = Pi / 180;

        // 向量所指方向对应的欧拉角(0-2pi)
        public static Fix64 Dir(this Vec2 v)
        {
            return MathEx.Atan(v.y, v.x);
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

        // 截断到给定范围
        public static Fix64 Clamp(this Fix64 v, Fix64 min, Fix64 max)
        {
            if (v < min)
                return min;
            else if (v > max)
                return max;
            else
                return v;
        }

        // 计算转向，从当前朝向转向目标方向，并限制最大转动角度
        public static Fix64 CalcDir4Turn2(Vec2 nowDir, Vec2 turn2Dir, Fix64 max)
        {
            max = max > 0 ? max : -max;
            var dirFrom = nowDir.Dir();
            var dirTo = turn2Dir.Dir();
            var tv = (dirTo - dirFrom).RangeInPi();
            if (Fix64.Abs(max) < Fix64.Abs(tv))
                return tv > 0 ? max : -max;
            else
                return tv;
        }

        // 将指定角度规范到 [-Pi, Pi)
        public static Fix64 RangeInPi(this Fix64 dir)
        {
            var d = dir;
            while (d > Pi)
                d -= Pi2;

            while (d < -Pi)
                d += Pi2;

            return d;
        }

        // 取绝对值
        public static Fix64 Abs(this Fix64 v)
        {
            return v >= 0 ? v : -v;
        }

        // 计算三角函数

        public static Fix64 Cos(Fix64 arc)
        {
            return Fix64.Cos(arc);
        }

        public static Fix64 Sin(Fix64 arc)
        {
            return Fix64.Sin(arc);
        }

        public static Fix64 Tan(Fix64 arc)
        {
            return Fix64.Tan(arc);
        }

        public static Fix64 Atan(Fix64 y, Fix64 x)
        {
            return Fix64.Atan2(y, x);
        }
    }
}
