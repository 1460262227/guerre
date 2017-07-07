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
        public override string Type => "Airplane";

        public int BulletNum = 0;

        public void UseSkill(string skillName)
        {
            switch(skillName)
            {
                case "gun":
                    AddBullet();
                    break;
            }
        }

        void AddBullet()
        {
            var b = new Bullet();
            BulletNum++;
            b.ID = ID + "/bullet_" + BulletNum;
            b.Pos = Pos + DirV2 * 0.3f;
            b.Dir = Dir;
            b.Velocity = 3;

            // b.Pos += DirV2.PerpendicularL * 0.1f;
            Room.AddObject(b);

            //b = new Bullet();
            //BulletNum++;
            //b.ID = ID + "/bullet_" + BulletNum;
            //b.Pos = Pos + DirV2 * 0.3f;
            //b.Dir = Dir;
            //b.Velocity = 3;

            //b.Pos += DirV2.PerpendicularL * -0.1f;
            //Room.AddObject(b);
        }
    }
}
