using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Guerre;
using System;

// 可移动的物体
public class MovableObject : MonoBehaviour
{
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
            return transform.localRotation.eulerAngles.z * Mathf.PI / 180;
        }
        set
        {
            transform.localRotation = Quaternion.Euler(0, 0, 180 * value / Mathf.PI);
        }
    }

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

    // 沿已规划路径或当前方向移动一段距离
    public void MoveForward(float te)
    {
        var dist = te * Velocity;

        // 更新角度
        if (TurnV != 0)
        {
            var dv = DirV2;
            var da = TurnV * te;
            var q = Quaternion.FromToRotation(new Vector3(dv.x, dv.y), new Vector3(Turn2Dir.x, Turn2Dir.y));
            var tv = q.eulerAngles.z * Mathf.PI / 180;
            if (tv > Mathf.PI)
            {
                tv -= Mathf.PI;
                da = -da;
            }

            da = Mathf.Abs(da) < Mathf.Abs(tv) ? da : tv;
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
    }
}
