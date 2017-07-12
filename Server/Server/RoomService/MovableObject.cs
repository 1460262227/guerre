using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;
using Swift.Math;
using Guerre;

namespace Server
{
    /// <summary>
    /// 房间内的可移动物体
    /// </summary>
    public abstract class MovableObject
    {
        public MovableObject()
        {
            Init();
        }

        // 唯一 ID
        public string ID;

        // 物体类型
        public abstract string Type { get; }

        // 移动速率
        public Fix64 Velocity { get; set; }

        // 最大角速度
        public Fix64 MaxTurnV { get; set; }

        // 角速度
        public Fix64 TurnV { get; set; }

        // 当前位置
        public Vec2 Pos { get; set; }

        // 当前方向(沿 x 正方向顺时针，弧度)
        public Fix64 Dir { get; set; }

        // 转向目标方向
        public Vec2 Turn2Dir { get; set; }

        // 可以移除了
        public bool ToBeRemoved { get; set; }

        // 尺寸半径
        public Fix64 Radius
        {
            get { return r; }
            set { r = value; r2 = r * r; }
        } Fix64 r;
        public Fix64 Radius2 { get { return r2; } } Fix64 r2;

        // 破坏力
        public Fix64 Power { get; set; }

        // 生命值
        public Fix64 MaxHp
        {
            get { return maxHp; }
            set { maxHp = value; }
        } Fix64 maxHp;

        // 生命值
        public Fix64 Hp
        {
            get { return hp; }
            set { hp = value; ToBeRemoved = ToBeRemoved || hp <= 0; }
        } Fix64 hp;

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
            MoveForward(te);
        }

        // 沿当前方向移动一段距离
        public virtual Fix64 MoveForward(Fix64 te)
        {
            // 更新角度
            if (TurnV != 0)
            {
                var da = MathEx.CalcDir4Turn2(DirV2, Turn2Dir, TurnV * te);
                Dir += da;
            }

            var d = te * Velocity;
            var dx = MathEx.Cos(Dir) * d;
            var dy = MathEx.Sin(Dir) * d;
            var dv = new Vec2(dx, dy);
            Pos += dv;

            return dv.Length;
        }

        public virtual void Init()
        {
            Pos = Vec2.Zero;
            Radius = 0.1f;
            MaxHp = Hp = 1;
            Dir = MathEx.HalfPi;
            Turn2Dir = Vec2.Zero;
            TurnV = 0;
            ToBeRemoved = false;
            Room = null;
        }

        // 判断碰撞情况
        public virtual bool CheckCollide(MovableObject obj)
        {
            return false;
        }

        // 执行碰撞操作
        public virtual bool DoCollide(MovableObject obj)
        {
            if (!CheckCollide(obj) && !obj.CheckCollide(this))
                return false;

            obj.Hp -= Power;
            Hp -= obj.Power;
            return true;
        }

        public GameRoom Room { get; set; }
    }
}
