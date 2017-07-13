using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Swift;
using Swift.Math;
using Guerre;

namespace Guerre
{
    /// <summary>
    /// 房间内的物体信息
    /// </summary>
    public class ObjectInfo : SerializableData
    {
        public ObjectInfo()
        {
            Init();
        }

        // 唯一 ID
        public string ID;

        // 物体类型
        public string Type;

        // 碰撞类型
        public string CollisionType;

        // 当前位置
        public Vec2 Pos;

        // 当前方向(沿 x 正方向顺时针，弧度)
        public Fix64 Dir;

        // 破坏力
        public Fix64 Power;

        // 半径
        Fix64 r;

        // 最大血量
        Fix64 maxHp;

        // 血量
        Fix64 hp;

        // 尺寸半径
        public Fix64 Radius2 { get { return r * r; } }
        public Fix64 Radius
        {
            get { return r; }
            set { r = value; }
        }

        // 生命值
        public Fix64 MaxHp
        {
            get { return maxHp; }
            set { maxHp = value; }
        }

        // 生命值
        public virtual Fix64 Hp
        {
            get { return hp; }
            set { hp = value; }
        }

        // 当前方向的 Vector2 表达
        public Vec2 DirV2
        {
            get
            {
                var dir = Dir;
                return new Vec2(MathEx.Cos(dir), MathEx.Sin(dir));
            }
            set { Dir = value.Dir(); }
        }

        public virtual void OnTimeElapsed(Fix64 te)
        {
        }

        public virtual void Init()
        {
            Type = null;
            CollisionType = null;
            Power = 0;
            Pos = Vec2.Zero;
            Radius = 0.1f;
            MaxHp = Hp = 1;
            Dir = MathEx.HalfPi;
        }

        protected override void Sync()
        {
            BeginSync();
            {
                SyncString(ref ID);
                SyncString(ref Type);
                SyncString(ref CollisionType);
                SyncVec2(ref Pos);
                SyncFix64(ref Dir);
                SyncFix64(ref Power);
                SyncFix64(ref r);
                SyncFix64(ref maxHp);
                SyncFix64(ref hp);
            }
            EndSync();
        }
    }
}
