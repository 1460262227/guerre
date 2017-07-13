using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using Swift;
using Swift.Math;
using Guerre;

/// <summary>
/// 可移动的物体
/// </summary>
public class MovableObjectController : MonoBehaviour
{
    public MovableObjectInfo MO { get; set; }

    // 唯一 ID
    public string ID { get { return MO.ID; } }

    // 移动速率
    public Fix64 Velocity { get { return MO.Velocity; } set { MO.Velocity = value; } }

    // 最大角速度
    public Fix64 MaxTurnV { get { return MO.MaxTurnV; } set { MO.MaxTurnV = value; } }

    // 角速度
    public virtual Fix64 TurnV
    {
        get { return MO.TurnV; }
        set { MO.TurnV = value; preTurnV = value; }
    }

    // 转向目标方向
    public Vec2 Turn2Dir
    {
        get { return MO.Turn2Dir; }
        set { MO.Turn2Dir = value; }
    }

    // 充能状态
    public bool Powering
    {
        get { return MO.powering; }
        set { MO.powering = value; }
    }

    // 当前位置
    public Vec2 Pos
    {
        get
        {
            return MO.Pos;
        }
        set
        {
            MO.Pos = value;
            PrePos = value;
        }
    }

    public Vec2 PrePos { set { prePos = value; } }
    Vec2 prePos;

    public virtual Fix64 Hp { get { return MO.Hp; } set { MO.Hp = value; } }
    public virtual Fix64 MaxHp { get { return MO.MaxHp; } set { MO.MaxHp = value; } }
    public virtual Fix64 Mp { get { return MO.Mp; } set { MO.Mp = value; } }
    public virtual Fix64 MaxMp { get { return MO.MaxMp; } set { MO.MaxMp = value; } }
    public virtual Fix64 Power { get { return MO.Power; } set { MO.Power = value; } }

    // 当前方向(沿 x 正方向顺时针，弧度)
    public Fix64 Dir
    {
        get
        {
            return MO.Dir;
        }
        set
        {
            MO.Dir = value;
            PreDir = value;
        }
    }

    public Fix64 PreDir { set { preDir = value; } }
    Fix64 preDir;

    // 当前方向的 vector2 表达
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

    public Vec2 PreDirV2
    {
        set
        {
            PreDir = value.Dir();
        }
    }

    public Fix64 PreTurnV
    {
        set
        {
            preTurnV = value;
        }
    } Fix64 preTurnV;

    public void UpdateImmediately()
    {
        ShowRotation = Quaternion.Euler(0, 0, (float)(Dir * MathEx.Rad2Deg));
        ShowPosition = new Vector3((float)Pos.x, (float)Pos.y, 0);
    }

    public virtual Quaternion ShowRotation { get { return transform.localRotation; } set { transform.localRotation = value; } }
    public virtual Vector3 ShowPosition { get { return transform.localPosition; } set { transform.localPosition = value; } }

    bool shirfting = false;
    public void UpdateSmoothly(float te)
    {
        // if (!shirfting)
        //    return;

        var nowDir = ShowRotation.eulerAngles.z * MathEx.Deg2Rad;
        var nowPos = ShowPosition;

        var dd = te * preTurnV;
        var dp = te * Velocity;

        var toDir = preDir;
        var toPos = new Vector3((float)prePos.x, (float)prePos.y, 0);

        var dirDist = (toDir - nowDir).RangeInPi();
        var dirDistAbs = dirDist.Abs();
        var posDist = (toPos - nowPos).magnitude;

        var dDiv = dd >= dirDistAbs ? 1 : dd / dirDistAbs;
        var pDiv = dp >= posDist ? 1 : dp / posDist;

        var d = dirDist * dDiv + nowDir;
        var p = (toPos - nowPos) * (float)pDiv + nowPos;

        shirfting = pDiv < 1 || dDiv < 1;

        ShowRotation = Quaternion.Euler(0, 0, (float) (d * MathEx.Rad2Deg));
        ShowPosition = new Vector3((float)p.x, (float)p.y, 0);
    }

    public void OnTimeElapsed(Fix64 te)
    {
        MO.OnTimeElapsed(te);
        Pos = MO.Pos;
        Dir = MO.Dir;
        TurnV = MO.TurnV;
    }
}
