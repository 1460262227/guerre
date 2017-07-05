﻿using System;
using System.Collections;
using System.Collections.Generic;
using Guerre;
using Swift;
using Swift.Math;

namespace Server
{
    // 一架飞机
    public class Airplane
    {
        // 唯一 ID
        public string ID;

        // 移动速率
        public float Velocity { get; set; }

        // 当前位置
        public Vec2 Pos { get; set; }

        // 当前方向(沿 x 正方向顺时针，弧度)
        public float Dir { get; set; }

        // 当前方向的 Vector2 表达
        public Vec2 DirV2
        {
            get
            {
                var dir = Dir;
                return new Vec2((float)Math.Cos(dir), (float)Math.Sin(dir));
            }
            set
            {
                Dir = value.Dir();
            }
        }

        // 沿当前方向移动一段距离
        public void MoveForward(float te)
        {
            var d = te * Velocity;
            var dx = (float)Math.Cos(Dir) * d;
            var dy = (float)Math.Sin(Dir) * d;
            Pos += new Vec2(dx, dy);
        }
    }
}
