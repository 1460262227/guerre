﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Swift;

namespace Guerre
{
    /// <summary>
    /// 执行碰撞逻辑
    /// </summary>
    public class Collider
    {
        // 执行检查碰撞
        public static bool CheckCollision(MovableObjectInfo obj1, MovableObjectInfo obj2)
        {
            return DoCollision(obj1, obj2, true);
        }

        // 执行碰撞逻辑
        public static bool DoCollision(MovableObjectInfo obj1, MovableObjectInfo obj2, bool onlyCheck = false)
        {
            if (obj1.CollisionType == null || obj2.CollisionType == null)
                return false;

            var ct1 = obj1.CollisionType;
            var ct2 = obj2.CollisionType;
            Func<bool> c = null;

            if (CM.ContainsKey(ct1) && CM[ct1].ContainsKey(ct2))
                c = () => CM[ct1][ct2](obj1, obj2, onlyCheck);
            else if (CM.ContainsKey(ct2) && CM[ct2].ContainsKey(ct1))
                c = () => CM[ct2][ct1](obj2, obj1, onlyCheck);

            if (c == null)
                return false;

            return c();
        }

        #region 各类型物体碰撞逻辑

        // 是否有区域重叠
        static bool IsOverlapped(MovableObjectInfo obj1, MovableObjectInfo obj2)
        {
            var pos1 = obj1.Pos;
            var pos2 = obj2.Pos;
            var d = (pos1 - pos2).Length;
            return d < obj1.Radius + obj2.Radius;
        }

        // 碰撞矩阵
        static Dictionary<string, Dictionary<string, Func<MovableObjectInfo, MovableObjectInfo, bool, bool>>> CM = new Dictionary<string, Dictionary<string, Func<MovableObjectInfo, MovableObjectInfo, bool, bool>>>();

        static Collider()
        {
            // bullet => airplane
            CM["Bullet"] = new Dictionary<string, Func<MovableObjectInfo, MovableObjectInfo, bool, bool>>();
            CM["Bullet"]["Airplane"] = (b, a, onlyCheck) =>
            {
                var overlapped = IsOverlapped(b, a);
                if (onlyCheck || !overlapped)
                    return overlapped;

                // 执行碰撞逻辑
                a.Hp -= b.Power;
                b.Hp = 0;

                return true;
            };

            // coin => airplane
            CM["Coin"] = new Dictionary<string, Func<MovableObjectInfo, MovableObjectInfo, bool, bool>>();
            CM["Coin"]["Airplane"] = (c, a, onlyCheck) =>
            {
                var overlapped = IsOverlapped(c, a);
                if (onlyCheck || !overlapped)
                    return overlapped;

                // 执行碰撞逻辑
                c.Hp = 0;

                return true;
            };

            // medicine => airplane
            CM["Medicine"] = new Dictionary<string, Func<MovableObjectInfo, MovableObjectInfo, bool, bool>>();
            CM["Medicine"]["Airplane"] = (m, a, onlyCheck) =>
            {
                var overlapped = IsOverlapped(m, a);
                if (onlyCheck || !overlapped)
                    return overlapped;

                // 执行碰撞逻辑
                if (a.Hp < a.MaxHp)
                    a.Hp = a.Hp + 1;
                m.Hp = 0;

                return true;
            };

            // lightning => airplane
            CM["Lightning"] = new Dictionary<string, Func<MovableObjectInfo, MovableObjectInfo, bool, bool>>();
            CM["Lightning"]["Airplane"] = (l, a, onlyCheck) =>
            {
                var overlapped = IsOverlapped(l, a);
                if (onlyCheck || !overlapped)
                    return overlapped;

                // 执行碰撞逻辑
                if (a.Mp < a.MaxHp)
                    a.Mp = a.Mp + 1;
                l.Hp = 0;

                return true;
            };
        }

        #endregion
    }
}
