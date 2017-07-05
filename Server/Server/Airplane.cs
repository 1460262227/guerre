using System.Collections;
using System.Collections.Generic;
using Guerre;

namespace Server
{
    // 一架飞机
    public class Airplane
    {
        // 唯一 ID
        public string ID;

        // 飞机类型
        public int Type = 0;

        // 攻击力
        public int Power = 1;

        // 耐久度
        public int Durability = 1;

        // 飞行速率
        public float Velocity = 1;

        // 剩余航行时间
        public float LifeTimeLeft = 5;

        // 碰撞半径
        public float Radius = 1;

        // 当前方向
        public Vec2 Dir { get; set; }

        // 当前位置
        public Vec2 Pos { get; set; }
    }
}
