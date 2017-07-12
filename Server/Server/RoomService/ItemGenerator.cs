using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Swift;
using Swift.Math;
using Guerre;

namespace Server
{
    /// <summary>
    /// 道具生成
    /// </summary>
    public class ItemGenerator : Component
    {
        // 物品生成空间范围
        public Vec2 GenSpaceLeftTop;
        public Vec2 GenSpaceSize;

        // 医药箱每秒生成密度
        public float MedicineDensity;

        int items = 0;

        // 随机生成物品
        public void RandomGen(Fix64 te, Action<MovableObject> cb)
        {
            // 医药箱
            if (Utils.RandomHit((float)te * MedicineDensity))
            {
                items++;
                var m = new Medicine();
                m.ID = "Medicine/" + items;
                m.Pos = GenSpaceLeftTop + Utils.RandomVec2(GenSpaceSize);
                m.Dir = 0;
                cb(m);
            }
        }
    }
}
