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

        // 每秒生成密度
        public float MedicineDensity; // 医药箱
        public float CoinDensity; // 金币
        public float LightningDensity; // 闪电

        int items = 0;

        Dictionary<Func<MovableObject>, float> gens = new Dictionary<Func<MovableObject>, float>();

        public void BuildGenDensity()
        {
            gens[() => new Medicine()] = MedicineDensity;
            gens[() => new Coin()] = CoinDensity;
            gens[() => new Lightning()] = LightningDensity;
        }

        // 随机生成物品
        public void RandomGen(Fix64 te, Action<MovableObject> cb)
        {
            foreach (var g in gens.Keys)
            {
                float r = gens[g] * (float)te;
                if (Utils.RandomHit(r))
                {
                    items++;
                    var obj = g();
                    obj.ID = "Lightning/" + items;
                    obj.Pos = GenSpaceLeftTop + Utils.RandomVec2(GenSpaceSize);
                    obj.Dir = 0;
                    cb(obj);
                }
            }
        }
    }
}
