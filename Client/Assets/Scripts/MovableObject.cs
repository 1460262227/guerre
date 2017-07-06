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
    public float Velocity { get; set; }

    // 角速度
    public float TurnV { get; set; }

    // 转向目标方向
    public Vec2 Turn2Dir { get; set; }

    // 当前位置
    public Vec2 Pos
    {
        get
        {
            var p = transform.localPosition;
            return new Vec2(p.x, p.y);
        }
        set
        {
            transform.localPosition = new Vector3(value.x, value.y, 0);
        }
    }

    // 当前方向(沿 x 正方向顺时针，弧度)
    public float Dir
    {
        get
        {
            return dir;
        }
        set
        {
            dir = value;
            transform.localRotation = Quaternion.Euler(0, 0, value * MathEx.Rad2Deg);
        }
    } float dir = 0;

    // 当前方向的 Vector2 表达
    public Vec2 DirV2
    {
        get
        {
            var dir = Dir;
            return new Vec2(Mathf.Cos(dir), Mathf.Sin(dir));
        }
        set
        {
            Dir = value.Dir();
        }
    }

    // 沿当前方向移动一段距离
    public void MoveForward(float te)
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
    public void MoveForwardOnDir(float d)
    {
        var dx = Mathf.Cos(Dir) * d;
        var dy = Mathf.Sin(Dir) * d;
        Pos += new Vec2(dx, dy);

        Debug.Log("pos = " + Pos.x + ", " + Pos.y);
    }
}
