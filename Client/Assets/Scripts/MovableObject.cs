using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using Swift;
using Swift.Math;

// 可移动的物体
public class MovableObject : MonoBehaviour
{
    // 唯一 ID
    public string ID { get; set; }

    // 移动速率
    public Fix64 Velocity { get; set; }

    // 角速度
    public Fix64 TurnV { get; set; }

    // 转向目标方向
    public Vec2 Turn2Dir { get; set; }

    // 当前位置
    public Vec2 Pos
    {
        get
        {
            return pos;
        }
        set
        {
            pos = value;
            transform.localPosition = new Vector3((float)value.x, (float)value.y, 0);
        }
    } Vec2 pos = Vec2.Zero;

    // 当前方向(沿 x 正方向顺时针，弧度)
    public Fix64 Dir
    {
        get
        {
            return dir;
        }
        set
        {
            dir = value;
            transform.localRotation = Quaternion.Euler(0, 0, (float)(value * MathEx.Rad2Deg));
        }
    } Fix64 dir = 0;

    // 当前方向的 Vector2 表达
    public Vec2 DirV2
    {
        get
        {
            var dir = Dir;
            return new Vec2(MathEx.Cos(dir), MathEx.Sin(dir));
        }
        set
        {
            Dir = value.Dir();
        }
    }

    // 沿当前方向移动一段距离
    public void MoveForward(Fix64 te)
    {
        var dist = te * Velocity;

        // 更新角度
        if (TurnV != 0)
        {
            var da = MathEx.CalcDir4Turn2(DirV2, Turn2Dir, te * TurnV);
            Dir += da;
        }

        MoveForwardOnDir(dist);
    }

    // 沿当前方向移动
    public void MoveForwardOnDir(Fix64 d)
    {
        var dx = MathEx.Cos(Dir) * d;
        var dy = MathEx.Sin(Dir) * d;
        Pos += new Vec2(dx, dy);
    }
}
