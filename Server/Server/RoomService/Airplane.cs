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

        public uint BulletNum = 0;

        public override void Init()
        {
            base.Init();

            Velocity = 1;
            MaxHp = 10;
            Hp = 10;
            Power = 10;
            Radius = 0.3f;
        }

        public void UseSkill(string skillName)
        {
            switch(skillName)
            {
                case "gun":
                    Shot();
                    break;
            }
        }

        // 机枪射击
        void Shot()
        {
            var b = new SmallBullet();
            BulletNum++;
            b.ID = ID + "/bullet_" + BulletNum;
            b.Pos = Pos + DirV2 * 0.35f;
            b.Dir = Dir;

            Room.AddObject(b);
        }
    }
}
