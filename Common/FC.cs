using System;

namespace Guerre
{
    // 流程控制工具(flow control)
    public static class FC
    {
        public static void For(int start, int end, Action<int> f)
        {
            for (var i = start; i < end; i++)
                f(i);
        }

        public static void For(int end, Action<int> f)
        {
            For(0, end, f);
        }
    }
}
