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
        public override string Type => airplaneType;

        uint BulletNum = 0;
        Action gunShot = null;

        string airplaneType = "Airplane_";
        public void BuildAttrs(int type)
        {
            MaxHp = 10;
            Hp = 10;
            Power = 10;
            Radius = 0.3f;
            airplaneType += type;

            switch (type)
            {
                case 0:
                    Velocity = 1;
                    MaxTurnV = 0.75f;
                    gunShot = DoubleShot;
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
            }
        }

        // 机枪射击
        SmallBullet MakeBullet(float leftOffset)
        {
            var b = new SmallBullet();
            BulletNum++;
            b.ID = ID + "/bullet_" + BulletNum;
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
        void DoubleShot()
        {
            var b1 = MakeBullet(0.2f);
            var b2 = MakeBullet(-0.2f);
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
    }
}
