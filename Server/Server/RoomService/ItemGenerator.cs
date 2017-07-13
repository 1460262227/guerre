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

        // 目前尚存的道具
        Dictionary<string, int> itemExistsCount = new Dictionary<string, int>();
        Dictionary<string, List<WeakReference<MovableObject>>> itemExists = new Dictionary<string, List<WeakReference<MovableObject>>>();

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
                var r = gens[g];
                float hit = r * (float)te;
                if (Utils.RandomHit(hit))
                {
                    var obj = g();
                    var type = obj.Type;

                    if (!itemExists.ContainsKey(type))
                        itemExists[type] = new List<WeakReference<MovableObject>>();

                    if (!itemExistsCount.ContainsKey(type))
                        itemExistsCount[type] = 0;

                    // 二次概率检查控制物品生成总数，每种不超过 10 个
                    var existsCnt = itemExistsCount[type];
                    if (existsCnt >= 10)
                        continue;

                    obj.ID = type + "/" + items;
                    obj.Pos = GenSpaceLeftTop + Utils.RandomVec2(GenSpaceSize);
                    obj.Dir = 0;

                    var lst = itemExists[type];
                    lst.Add(new WeakReference<MovableObject>(obj));

                    items++;
                    cb(obj);
                }
            }
        }

        // 重新统计道具数量
        public void RefreshItemStatistics()
        {
            foreach (var k in itemExists.Keys)
            {
                var lst = itemExists[k];
                foreach (var r in lst.ToArray())
                {
                    MovableObject obj = null;
                    if (!r.TryGetTarget(out obj))
                        lst.Remove(r);
                }

                itemExistsCount[k] = lst.Count;
            }
        }
    }
}
