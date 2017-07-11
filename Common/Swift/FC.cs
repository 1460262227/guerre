using System;

namespace Swift
{
    // 流程控制工具(flow control)
    public static class FC
    {
        public static void For(int start, int end, Action<int> f, Func<bool> interupt = null)
        {
            for (var i = start; i < end && (interupt == null || !interupt()); i++)
                f(i);
        }

        public static void For(int end, Action<int> f, Func<bool> interupt = null)
        {
            For(0, end, f, interupt);
        }
    }
}
