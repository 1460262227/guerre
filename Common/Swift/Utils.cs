using System;
using System.Collections.Generic;
using System.Text;

namespace Swift
{
    public partial class Utils
    {
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
    }
}
