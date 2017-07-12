using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
