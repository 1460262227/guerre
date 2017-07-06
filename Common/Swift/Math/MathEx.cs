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

        public const float Rad2Deg = 180 / Pi;
        public const float Deg2Rad = Pi / 180;

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

        // 计算转向，从当前朝向转向目标方向，并限制最大转动角度
        public static float CalcDir4Turn2(Vec2 nowLookAt, Vec2 turn2LookAt, float max)
        {
            var dv = nowLookAt;
            var da = max;
            var q = Quat.FromToRotation(new Vec3(dv.x, dv.y, 0), new Vec3(turn2LookAt.x, turn2LookAt.y, 0));
            // var qq = UnityEngine.Quaternion.FromToRotation(new UnityEngine.Vector3(dv.x, dv.y, 0), new UnityEngine.Vector3(turn2LookAt.x, turn2LookAt.y, 0));

            var tv = q.eulerAngles.z * MathEx.Deg2Rad;
            if (tv > MathEx.Pi)
            {
                tv -= MathEx.Pi;
                da = -da;
            }

            da = (float)System.Math.Abs(da) < (float)System.Math.Abs(tv) ? da : tv;
            return da;
        }
    }
}
