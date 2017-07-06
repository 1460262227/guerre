using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Guerre
{
    public delegate void Action<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

    public static class Ext
    {
        public static void SC(this Action act)
        {
            if (act != null)
                act();
        }

        public static void SC<T>(this Action<T> act, T p)
        {
            if (act != null)
                act(p);
        }

        public static void SC<T1, T2>(this Action<T1, T2> act, T1 p1, T2 p2)
        {
            if (act != null)
                act(p1, p2);
        }

        public static void SC<T1, T2, T3>(this Action<T1, T2, T3> act, T1 p1, T2 p2, T3 t3)
        {
            if (act != null)
                act(p1, p2, t3);
        }

        public static void SC<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> act, T1 p1, T2 p2, T3 t3, T4 t4)
        {
            if (act != null)
                act(p1, p2, t3, t4);
        }

        public static void SC<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> act, T1 p1, T2 p2, T3 t3, T4 t4, T5 t5)
        {
            if (act != null)
                act(p1, p2, t3, t4, t5);
        }
    }
}
