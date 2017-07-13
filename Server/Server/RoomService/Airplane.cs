using System;
using System.Collections;
using System.Collections.Generic;
using Guerre;
using Swift;
using Swift.Math;

namespace Server
{
    // 飞机
    public class Airplane : MovableObject
    {
        uint BulletNum = 0;
        Action gunShot = null;

        Action powerUp = null;
        Action powerDown = null;

        public override void Init()
        {
            base.Init();
            CollisionType = "Airplane";
        }

        public void BuildAttrs(int type)
        {
            MaxHp = 10;
            Hp = 10;
            MaxMp = 10;
            Mp = 5;
            Power = 10;
            Radius = 0.3f;
            Type = "Airplane/" + type;
            var p = new Vec2(Utils.RandomFloat(-3, 3), Utils.RandomFloat(-3, 3));
            Pos = p;
            p.Normalize();
            Dir = p.Dir() + MathEx.Pi;

            switch (type)
            {
                case 0:
                    Velocity = 1;
                    MaxTurnV = 0.75f;
                    gunShot = DoubleShoot;
                    powerUp = SpeedUp;
                    powerDown = SpeedDown;
                    break;
                case 1:
                    Velocity = 1.25f;
                    MaxTurnV = 1.25f;
                    gunShot = Shot;
                    break;
                case 2:
                    Velocity = 1;
                    MaxTurnV = 1;
                    gunShot = LongShot;
                    break;
            }
        }

        public void UseSkill(string skillName)
        {
            switch(skillName)
            {
                case "gun":
                    gunShot();
                    break;
                case "power":
                    powerUp();
                    break;
            }
        }

        public override void OnTimeElapsed(Fix64 te)
        {
            base.OnTimeElapsed(te);
            if (powering && Mp <= 0)
                powerDown();
        }

        // 机枪射击
        SmallBullet MakeBullet(float leftOffset)
        {
            BulletNum++;
            var b = new SmallBullet();
            b.ID = ID + "/bullet/" + BulletNum;
            b.OwnerID = ID;
            b.Pos = Pos + DirV2 * 0.35f + DirV2.PerpendicularL * leftOffset;
            b.Dir = Dir;
            b.RangeLeft = 3;
            b.Velocity = 3;

            return b;
        }

        // 单发
        void Shot()
        {
            var b = MakeBullet(0);
            Room.AddObject(b);
        }
        
        // 双发
        void DoubleShoot()
        {
            var b1 = MakeBullet(0.2f);
            var b2 = MakeBullet(-0.2f);
            b2.Dir = (DirV2 - DirV2.PerpendicularL * 0.1f).Dir();
            b1.Dir = (DirV2 + DirV2.PerpendicularL * 0.1f).Dir();
            Room.AddObject(b1);
            Room.AddObject(b2);
        }

        // 长距离
        void LongShot()
        {
            var b = MakeBullet(0);
            b.Velocity = 5;
            b.RangeLeft = 5;
            Room.AddObject(b);
        }

        // 加速
        void SpeedUp()
        {
            powering = true;
            Velocity *= 2;
            MaxTurnV *= 2;
            TurnV *= 2;
            Room.Boardcast("SpeedUp", (buff) => { buff.Write(ID); });
        }

        // 加速结束
        void SpeedDown()
        {
            powering = false;
            Velocity /= 2;
            MaxTurnV /= 2;
            TurnV /= 2;
            Room.Boardcast("SpeedDown", (buff) => { buff.Write(ID); });
        }
    }
}
