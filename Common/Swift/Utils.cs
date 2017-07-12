using System;
using System.Collections.Generic;
using System.Text;
using Swift.Math;

namespace Swift
{
    public partial class Utils
    {
        static Random rand = new Random();

        // 当前时间以好毫秒为单位
        public static long Now
        {
            get { return DateTime.Now.Ticks / 10000; }
        }

        // 当前时间以秒为单位
        public static long NowSecs
        {
            get { return Now / 1000; }
        }

        // 随机数

        public static int Random(int min, int max)
        {
            return rand.Next(min, max);
        }

        public static float RandomFloat(float min, float max)
        {
            var d = max - min;
            var r = rand.NextDouble();
            return (float)(r * d + min);
        }

        // 随机测试是否命中指定几率[0-1]
        public static bool RandomHit(float r)
        {
            return RandomFloat(0, 1) < r;
        }

        // 随机一个指定范围内的 Vec2
        public static Vec2 RandomVec2(Vec2 size)
        {
            var x = RandomFloat(0, (float)size.x);
            var y = RandomFloat(0, (float)size.y);
            return new Vec2(x, y);
        }
    }
}
