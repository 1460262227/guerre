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
    public Fix64 TurnV
    {
        get { return turnV; }
        set { turnV = value; preTurnV = value; }
    } Fix64 turnV;

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
            prePos = value;
        }
    } Vec2 pos = Vec2.Zero;

    public Vec2 PrePos
    {
        get { return prePos; }
        set { prePos = value; }
    } Vec2 prePos;

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
            preDir = value;
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

    public Fix64 PreDir
    {
        set
        {
            preDir = value;
        }
    }  Fix64 preDir = 0;

    public Vec2 PreDirV2
    {
        set
        {
            preDir = value.Dir();
        }
    }

    public Fix64 PreTurnV
    {
        set
        {
            preTurnV = value;
        }
    } Fix64 preTurnV;

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

        shirfting = true;
    }

    // 沿当前方向移动
    public void MoveForwardOnDir(Fix64 d)
    {
        var dx = MathEx.Cos(Dir) * d;
        var dy = MathEx.Sin(Dir) * d;
        Pos += new Vec2(dx, dy);
    }

    public void UpdateImmediately()
    {
        transform.localRotation = Quaternion.Euler(0, 0, (float)(dir * MathEx.Rad2Deg));
        transform.localPosition = new Vector3((float)pos.x, (float)pos.y, 0);
    }

    bool shirfting = false;
    public void UpdateSmoothly(float te)
    {
        if (!shirfting)
            return;

        var nowDir = transform.localRotation.eulerAngles.z;
        var nowPos = transform.localPosition;

        var dd = te * (float)preTurnV;
        var dp = te * (float)Velocity;

        var toDir = (float)(preDir);
        var toPos = new Vector3((float)prePos.x, (float)prePos.y, 0);

        var dirDist = toDir - nowDir;
        var posDist = (toPos - nowPos).magnitude;

        var dDiv = dd >= dirDist ? 1 : dd / dirDist;
        var pDiv = dp >= posDist ? 1 : dp / posDist;

        var d = (toDir - nowDir) * dDiv + nowDir;
        var p = (toPos - nowPos) * pDiv + nowPos;

        shirfting = pDiv < 1 || dDiv < 1;

        transform.localRotation = Quaternion.Euler(0, 0, (float) (d * MathEx.Rad2Deg));
        transform.localPosition = new Vector3((float)p.x, (float)p.y, 0);
    }
}
