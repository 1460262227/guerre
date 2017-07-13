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
    /// 房间内的可移动物体信息
    /// </summary>
    public class MovableObjectInfo : SerializableData
    {
        public MovableObjectInfo()
        {
            Init();
        }

        // 唯一 ID
        public string ID;

        // 物体类型
        public string Type;

        // 等级
        public int Level;

        // 碰撞类型
        public string CollisionType;

        // 移动速率
        public Fix64 Velocity;

        // 最大移动速率
        public Fix64 MaxVelocity;

        // 最大角速度
        public Fix64 MaxTurnV;

        // 角速度
        public Fix64 TurnV;

        // 当前位置
        public Vec2 Pos ;

        // 当前方向(沿 x 正方向顺时针，弧度)
        public Fix64 Dir;

        // 要专向的方向
        public Vec2 Turn2Dir;

        // 破坏力
        public Fix64 Power;

        // 剩余盾值
        public Fix64 Sheild;

        // 剩余加速值
        public Fix64 Speeding;

        // 半径
        Fix64 r;

        // 最大血量
        Fix64 maxHp;

        // 血量
        Fix64 hp;

        // 最大能量
        Fix64 maxMp;

        // 能量
        Fix64 mp;

        // 尺寸半径
        public Fix64 Radius2 { get { return r * r; } }
        public Fix64 Radius
        {
            get { return r; }
            set { r = value; }
        }

        // 最大生命值
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

        // 能量值
        public virtual Fix64 Mp
        {
            get { return mp; }
            set { mp = value; }
        }

        // 最大能量值
        public virtual Fix64 MaxMp
        {
            get { return maxMp; }
            set { maxMp = value; }
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

        public virtual void ProcessLogic(Fix64 te)
        {
            // 减加速状态
            if (Speeding > 0)
            {
                Speeding -= te;
                if (Speeding <= 0)
                    SpeedDown();
            }

            // 减盾
            if (Sheild > 0)
                Sheild -= te;
        }

        public virtual void ProcessMove(Fix64 te)
        {
            MoveForward(te);
        }

        // 沿当前方向移动一段距离
        public Fix64 MoveForward(Fix64 te)
        {
            var dist = te * Velocity;

            // 更新角度
            if (TurnV != 0)
            {
                var da = MathEx.CalcDir4Turn2(DirV2, Turn2Dir, te * TurnV);
                Dir += da;
            }

            MoveForwardOnDir(dist);
            return dist;
        }

        // 沿当前方向移动
        public void MoveForwardOnDir(Fix64 d)
        {
            var dx = MathEx.Cos(Dir) * d;
            var dy = MathEx.Sin(Dir) * d;
            Pos += new Vec2(dx, dy);
        }

        public virtual void Init()
        {
            Level = 0;
            Type = null;
            CollisionType = null;
            Power = 0;
            Pos = Vec2.Zero;
            Radius = 0.1f;
            MaxHp = Hp = 1;
            Dir = MathEx.HalfPi;
            Turn2Dir = Vec2.Zero;
            TurnV = 0;
        }

        protected override void Sync()
        {
            BeginSync();
            {
                SyncString(ref ID);
                SyncInt(ref Level);
                SyncString(ref Type);
                SyncString(ref CollisionType);
                SyncFix64(ref MaxVelocity);
                SyncFix64(ref Velocity);
                SyncFix64(ref MaxTurnV);
                SyncFix64(ref TurnV);
                SyncVec2(ref Pos);
                SyncFix64(ref Dir);
                SyncVec2(ref Turn2Dir);
                SyncFix64(ref Power);
                SyncFix64(ref r);
                SyncFix64(ref Sheild);
                SyncFix64(ref Speeding);
                SyncFix64(ref maxHp);
                SyncFix64(ref hp);
                SyncFix64(ref maxMp);
                SyncFix64(ref mp);
            }
            EndSync();
        }

        public void SpeedUp()
        {
            if (Mp <= 0)
                return;

            Speeding = Mp;
            Velocity *= 2;
            MaxTurnV *= 2;
            Turn2Dir *= 2;
        }

        public void SpeedDown()
        {
            Velocity /= 2;
            MaxTurnV /= 2;
            Turn2Dir /= 2;
        }
    }
}
