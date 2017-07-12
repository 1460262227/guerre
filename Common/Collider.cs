using System;
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
        public static bool CheckCollision()
        {
            return false;
        }

        // 执行碰撞逻辑
        public static bool DoCollision()
        {
            if (!CheckCollision())
                return false;

            return true;
        }
    }
}
